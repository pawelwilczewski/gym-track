using Application.Persistence;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Workout.Exercise.Set.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class DeleteWorkoutExerciseSetCommand(
	Id<Domain.Models.Workout.Workout> WorkoutId,
	int ExerciseIndex,
	int SetIndex,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class DeleteWorkoutExerciseSetHandler
	: IRequestHandler<DeleteWorkoutExerciseSetCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public DeleteWorkoutExerciseSetHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		DeleteWorkoutExerciseSetCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Modifiable
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.Sets)
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return new NotFound();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == request.ExerciseIndex);
		if (exercise is null) return new NotFound();

		var set = exercise.Sets.FirstOrDefault(set => set.Index == request.SetIndex);
		if (set is null) return new NotFound();

		exercise.Sets.Remove(set);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
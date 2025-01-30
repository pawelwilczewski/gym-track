using Application.Persistence;
using Domain.Models.Workout;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Workout.Exercise.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class DeleteWorkoutExerciseCommand(
	WorkoutId WorkoutId,
	int ExerciseIndex,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class DeleteWorkoutExerciseHandler
	: IRequestHandler<DeleteWorkoutExerciseCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public DeleteWorkoutExerciseHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		DeleteWorkoutExerciseCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Modifiable
			.Include(workout => workout.Exercises)
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return new NotFound();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == request.ExerciseIndex);
		if (exercise is null) return new NotFound();

		workout.Exercises.Remove(exercise);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
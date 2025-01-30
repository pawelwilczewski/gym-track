using Application.Persistence;
using Domain.Common.Results;
using Domain.Models;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Workout.Exercise.Set.Commands;

using ResultType = OneOf<Success, NotFound, ValidationError>;

public sealed record class UpdateWorkoutExerciseSetCommand(
	WorkoutId WorkoutId,
	WorkoutExerciseIndex ExerciseIndex,
	WorkoutExerciseSetIndex SetIndex,
	ExerciseMetric Metric,
	Reps Reps,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class UpdateWorkoutExerciseSetHandler
	: IRequestHandler<UpdateWorkoutExerciseSetCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public UpdateWorkoutExerciseSetHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		UpdateWorkoutExerciseSetCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Modifiable
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.ExerciseInfo)
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.Sets)
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return new NotFound();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == request.ExerciseIndex);
		if (exercise is null) return new NotFound();

		var set = exercise.Sets.FirstOrDefault(set => set.Index == request.SetIndex);
		if (set is null) return new NotFound();

		if (!set.TryUpdate(request.Metric, request.Reps, request.UserId, out var error)) return error.Value;

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
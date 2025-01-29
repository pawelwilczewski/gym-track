using Application.Persistence;
using Application.Workout.Exercise.Set.Dtos;
using Domain.Models.Workout;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Workout.Exercise.Set.Queries;

using ResultType = OneOf<Success<GetWorkoutExerciseSetResponse>, NotFound>;

public sealed record class GetWorkoutExerciseSetQuery(
	WorkoutId WorkoutId,
	int ExerciseIndex,
	int SetIndex,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class GetWorkoutExerciseSetHandler
	: IRequestHandler<GetWorkoutExerciseSetQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public GetWorkoutExerciseSetHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		GetWorkoutExerciseSetQuery request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Readable
			.AsNoTrackingWithIdentityResolution()
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.Sets)
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return new NotFound();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == request.ExerciseIndex);
		if (exercise is null) return new NotFound();

		var set = exercise.Sets.FirstOrDefault(set => set.Index == request.SetIndex);
		if (set is null) return new NotFound();

		return new Success<GetWorkoutExerciseSetResponse>(new GetWorkoutExerciseSetResponse(
			set.Index,
			set.Metric,
			set.Reps.Value,
			set.DisplayOrder));
	}
}
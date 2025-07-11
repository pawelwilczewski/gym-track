using Application.Persistence;
using Application.Workout.Exercise.Dtos;
using Domain.Common.Results;
using Domain.Models.User;
using Domain.Models.Workout;
using FuncNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Workout.Exercise.Queries;

using ResultType = Result<Success<GetWorkoutExerciseResponse>, NotFound>;

public sealed record class GetWorkoutExerciseQuery(
	WorkoutId WorkoutId,
	int ExerciseIndex,
	UserId UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class GetWorkoutExerciseHandler
	: IRequestHandler<GetWorkoutExerciseQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public GetWorkoutExerciseHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		GetWorkoutExerciseQuery request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Readable
			.AsNoTrackingWithIdentityResolution()
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.Sets)
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken);

		if (workout is null) return new NotFound();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == request.ExerciseIndex);
		if (exercise is null) return new NotFound();

		return new Success<GetWorkoutExerciseResponse>(new GetWorkoutExerciseResponse(
			exercise.Index.Value,
			exercise.ExerciseInfoId.Value,
			exercise.DisplayOrder,
			exercise.Sets
				.Select(set => new WorkoutExerciseSetKey(exercise.WorkoutId.Value, exercise.Index.Value, set.Index.Value))
				.ToList()));
	}
}
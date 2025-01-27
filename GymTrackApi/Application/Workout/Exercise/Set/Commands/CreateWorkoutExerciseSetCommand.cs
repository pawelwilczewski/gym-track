using Application.Persistence;
using Application.Workout.Exercise.Set.Dtos;
using Domain.Common;
using Domain.Common.Results;
using Domain.Models;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Workout.Exercise.Set.Commands;

using ResultType = OneOf<Success<GetWorkoutExerciseSetResponse>, NotFound, ValidationError>;

public sealed record class CreateWorkoutExerciseSetCommand(
	Id<Domain.Models.Workout.Workout> WorkoutId,
	int ExerciseIndex,
	ExerciseMetric Metric,
	PositiveCount Reps,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class CreateWorkoutExerciseSetHandler
	: IRequestHandler<CreateWorkoutExerciseSetCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public CreateWorkoutExerciseSetHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		CreateWorkoutExerciseSetCommand request,
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

		var index = exercise.Sets.GetNextIndex();
		var displayOrder = exercise.Sets.GetNextDisplayOrder();
		if (!Domain.Models.Workout.Workout.Exercise.Set.TryCreate(
			exercise, index, request.Metric, request.Reps, displayOrder, request.UserId, out var set, out var error))
		{
			return error.Value;
		}

		exercise.Sets.Add(set);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<GetWorkoutExerciseSetResponse>(new GetWorkoutExerciseSetResponse(
			set.Index,
			set.Metric,
			set.Reps.IntValue,
			set.DisplayOrder));
	}
}
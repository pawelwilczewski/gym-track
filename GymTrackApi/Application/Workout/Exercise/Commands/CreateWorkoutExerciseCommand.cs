using Application.Persistence;
using Application.Workout.Exercise.Dtos;
using Domain.Common;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Workout.Exercise.Commands;

using ResultType = OneOf<Success<GetWorkoutExerciseResponse>, NotFound>;

public sealed record class CreateWorkoutExerciseCommand(
	WorkoutId WorkoutId,
	ExerciseInfoId ExerciseInfoId,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class CreateWorkoutExerciseHandler
	: IRequestHandler<CreateWorkoutExerciseCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public CreateWorkoutExerciseHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		CreateWorkoutExerciseCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Modifiable
			.Include(workout => workout.Exercises)
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return new NotFound();

		var exerciseInfo = await dataContext.ExerciseInfos.Readable
			.AsNoTrackingWithIdentityResolution()
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == request.ExerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return new NotFound();

		var index = workout.Exercises.GetNextIndex();
		var displayOrder = workout.Exercises.GetNextDisplayOrder();
		var exercise = new WorkoutExercise(request.WorkoutId, index, request.ExerciseInfoId, displayOrder);

		workout.AddExercise(exercise, request.UserId);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<GetWorkoutExerciseResponse>(new GetWorkoutExerciseResponse(
			exercise.Index.Value,
			exercise.ExerciseInfoId.Value,
			exercise.DisplayOrder,
			exercise.Sets
				.Select(set => new WorkoutExerciseSetKey(exercise.WorkoutId.Value, exercise.Index.Value, set.Index.Value))
				.ToList()));
	}
}
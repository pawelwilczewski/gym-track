using Application.Persistence;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Workout.Exercise.DisplayOrder.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class UpdateWorkoutExerciseDisplayOrderQuery(
	Id<Domain.Models.Workout.Workout> WorkoutId,
	int ExerciseIndex,
	int DisplayOrder,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class UpdateWorkoutExerciseDisplayOrderHandler
	: IRequestHandler<UpdateWorkoutExerciseDisplayOrderQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public UpdateWorkoutExerciseDisplayOrderHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		UpdateWorkoutExerciseDisplayOrderQuery request,
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

		exercise.DisplayOrder = request.DisplayOrder;

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
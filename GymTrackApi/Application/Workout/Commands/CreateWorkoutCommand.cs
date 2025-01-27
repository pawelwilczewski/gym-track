using Application.Persistence;
using Application.Workout.Dtos;
using Domain.Common.ValueObjects;
using MediatR;
using OneOf.Types;

namespace Application.Workout.Commands;

using ResultType = Success<GetWorkoutResponse>;

public sealed record class CreateWorkoutCommand(
	Name Name,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class CreateWorkoutHandler
	: IRequestHandler<CreateWorkoutCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public CreateWorkoutHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		CreateWorkoutCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = Domain.Models.Workout.Workout.CreateForUser(request.Name, request.UserId);

		dataContext.Workouts.Add(workout);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<GetWorkoutResponse>(new GetWorkoutResponse(
			workout.Id.Value,
			workout.Name.ToString(),
			workout.Exercises.Select(exercise => new WorkoutExerciseKey(workout.Id.Value, exercise.Index))
				.ToList()));
	}
}
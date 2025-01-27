using Application.Persistence;
using Application.Workout.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;

namespace Application.Workout.Queries;

using ResultType = Success<List<GetWorkoutResponse>>;

public sealed record class GetWorkoutsQuery(
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class GetWorkoutsHandler
	: IRequestHandler<GetWorkoutsQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public GetWorkoutsHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		GetWorkoutsQuery request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workouts = dataContext.Workouts.Readable
			.AsNoTracking()
			.Select(workout => new GetWorkoutResponse(
				workout.Id.Value,
				workout.Name.ToString(),
				workout.Exercises.Select(exercise => new WorkoutExerciseKey(workout.Id.Value, exercise.Index)).ToList()));

		return new Success<List<GetWorkoutResponse>>(await workouts
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false));
	}
}
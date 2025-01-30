using Application.Persistence;
using Application.Tracking.TrackedWorkout.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;

namespace Application.Tracking.TrackedWorkout.Queries;

using ResultType = Success<List<GetTrackedWorkoutResponse>>;

public sealed record class GetTrackedWorkoutsQuery(
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class GetTrackedWorkoutsHandler
	: IRequestHandler<GetTrackedWorkoutsQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public GetTrackedWorkoutsHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		GetTrackedWorkoutsQuery request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var trackedWorkouts = dataContext.TrackedWorkouts.Readable
			.AsNoTrackingWithIdentityResolution()
			.Select(trackedWorkout => new GetTrackedWorkoutResponse(
				trackedWorkout.Id.Value,
				trackedWorkout.WorkoutId.Value,
				trackedWorkout.PerformedAt,
				trackedWorkout.Duration));

		return new Success<List<GetTrackedWorkoutResponse>>(await trackedWorkouts
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false));
	}
}
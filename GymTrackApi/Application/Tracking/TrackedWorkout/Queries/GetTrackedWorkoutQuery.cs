using Application.Persistence;
using Application.Tracking.TrackedWorkout.Dtos;
using Domain.Models.Tracking;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Tracking.TrackedWorkout.Queries;

using ResultType = OneOf<Success<GetTrackedWorkoutResponse>, NotFound>;

public sealed record class GetTrackedWorkoutQuery(
	TrackedWorkoutId TrackedWorkoutId,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class GetTrackedWorkoutHandler
	: IRequestHandler<GetTrackedWorkoutQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public GetTrackedWorkoutHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		GetTrackedWorkoutQuery request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var trackedWorkout = await dataContext.TrackedWorkouts.Readable
			.AsNoTrackingWithIdentityResolution()
			.FirstOrDefaultAsync(workout => workout.Id == request.TrackedWorkoutId, cancellationToken);

		if (trackedWorkout is null) return new NotFound();

		return new Success<GetTrackedWorkoutResponse>(new GetTrackedWorkoutResponse(
			trackedWorkout.Id.Value,
			trackedWorkout.WorkoutId.Value,
			trackedWorkout.PerformedAt,
			trackedWorkout.Duration));
	}
}
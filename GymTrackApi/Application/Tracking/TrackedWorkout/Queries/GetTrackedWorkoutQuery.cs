using Application.Persistence;
using Application.Tracking.TrackedWorkout.Dtos;
using Domain.Common.Results;
using Domain.Models.Tracking;
using Domain.Models.User;
using FuncNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tracking.TrackedWorkout.Queries;

using ResultType = Result<Success<GetTrackedWorkoutResponse>, NotFound>;

public sealed record class GetTrackedWorkoutQuery(
	TrackedWorkoutId TrackedWorkoutId,
	UserId UserId) : IRequest<ResultType>;

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
using Application.Persistence;
using Application.Tracking.TrackedWorkout.Dtos;
using Domain.Models.Workout;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Tracking.TrackedWorkout.Commands;

using ResultType = OneOf<Success<GetTrackedWorkoutResponse>, NotFound>;

public sealed record class CreateTrackedWorkoutCommand(
	WorkoutId WorkoutId,
	DateTime PerformedAt,
	TimeSpan Duration,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class CreateTrackedWorkoutHandler
	: IRequestHandler<CreateTrackedWorkoutCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public CreateTrackedWorkoutHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		CreateTrackedWorkoutCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Readable
			.AsNoTrackingWithIdentityResolution()
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken);

		if (workout is null) return new NotFound();

		var trackedWorkout = new Domain.Models.Tracking.TrackedWorkout(
			request.WorkoutId,
			request.PerformedAt,
			request.Duration,
			request.UserId);

		dataContext.TrackedWorkouts.Add(trackedWorkout);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<GetTrackedWorkoutResponse>(new GetTrackedWorkoutResponse(
			trackedWorkout.Id.Value,
			trackedWorkout.WorkoutId.Value,
			trackedWorkout.PerformedAt,
			trackedWorkout.Duration));
	}
}
using Application.Persistence;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Tracking.TrackedWorkout.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class UpdateTrackedWorkoutCommand(
	Id<Domain.Models.Tracking.TrackedWorkout> TrackedWorkoutId,
	DateTime PerformedAt,
	TimeSpan Duration,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class UpdateTrackedWorkoutHandler
	: IRequestHandler<UpdateTrackedWorkoutCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public UpdateTrackedWorkoutHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		UpdateTrackedWorkoutCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var trackedWorkout = await dataContext.TrackedWorkouts.Modifiable
			.FirstOrDefaultAsync(trackedWorkout => trackedWorkout.Id == request.TrackedWorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (trackedWorkout is null) return new NotFound();

		trackedWorkout.Update(request.PerformedAt, request.Duration, request.UserId);

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return new Success();
	}
}
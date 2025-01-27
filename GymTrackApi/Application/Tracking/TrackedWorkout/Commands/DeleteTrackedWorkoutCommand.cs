using Application.Persistence;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Tracking.TrackedWorkout.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class DeleteTrackedWorkoutCommand(
	Id<Domain.Models.Tracking.TrackedWorkout> TrackedWorkoutId,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class DeleteTrackedWorkoutHandler
	: IRequestHandler<DeleteTrackedWorkoutCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public DeleteTrackedWorkoutHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		DeleteTrackedWorkoutCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var trackedWorkout = await dataContext.TrackedWorkouts.Modifiable
			.FirstOrDefaultAsync(trackedWorkout => trackedWorkout.Id == request.TrackedWorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (trackedWorkout == null) return new NotFound();

		dataContext.TrackedWorkouts.Remove(trackedWorkout);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
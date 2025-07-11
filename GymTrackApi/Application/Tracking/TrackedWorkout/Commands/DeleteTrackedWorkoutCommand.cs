using Application.Persistence;
using Domain.Common.Results;
using Domain.Models.Tracking;
using Domain.Models.User;
using FuncNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tracking.TrackedWorkout.Commands;

using ResultType = Result<Success, NotFound>;

public sealed record class DeleteTrackedWorkoutCommand(
	TrackedWorkoutId TrackedWorkoutId,
	UserId UserId) : IRequest<ResultType>;

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
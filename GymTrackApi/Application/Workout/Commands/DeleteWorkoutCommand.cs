using Application.Persistence;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Workout.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class DeleteWorkoutCommand(
	Id<Domain.Models.Workout.Workout> WorkoutId,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class DeleteWorkoutHandler
	: IRequestHandler<DeleteWorkoutCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public DeleteWorkoutHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		DeleteWorkoutCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Modifiable
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return new NotFound();

		dataContext.Workouts.Remove(workout);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
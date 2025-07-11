using Application.Persistence;
using Domain.Common.Results;
using Domain.Models.User;
using Domain.Models.Workout;
using FuncNet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Workout.Commands;

using ResultType = Result<Success, NotFound>;

public sealed record class DeleteWorkoutCommand(
	WorkoutId WorkoutId,
	UserId UserId) : IRequest<ResultType>;

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
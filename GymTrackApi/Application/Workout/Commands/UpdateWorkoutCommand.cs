using Application.Persistence;
using Domain.Common.ValueObjects;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.Workout.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class UpdateWorkoutCommand(
	Id<Domain.Models.Workout.Workout> WorkoutId,
	Name Name,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class UpdateWorkoutHandler
	: IRequestHandler<UpdateWorkoutCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public UpdateWorkoutHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		UpdateWorkoutCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var workout = await dataContext.Workouts.Modifiable
			.FirstOrDefaultAsync(workout => workout.Id == request.WorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return new NotFound();

		workout.Update(request.Name, request.UserId);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
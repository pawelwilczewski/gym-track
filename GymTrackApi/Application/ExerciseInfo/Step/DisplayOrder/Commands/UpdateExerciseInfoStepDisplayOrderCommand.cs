using Application.Persistence;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.ExerciseInfo.Step.DisplayOrder.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class UpdateExerciseInfoStepDisplayOrderCommand(
	Id<Domain.Models.ExerciseInfo.ExerciseInfo> ExerciseInfoId,
	int StepIndex,
	int DisplayOrder,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class UpdateExerciseInfoStepDisplayOrderHandler
	: IRequestHandler<UpdateExerciseInfoStepDisplayOrderCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public UpdateExerciseInfoStepDisplayOrderHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		UpdateExerciseInfoStepDisplayOrderCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var exerciseInfo = await dataContext.ExerciseInfos.Modifiable
			.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == request.StepIndex))
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == request.ExerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return new NotFound();

		var step = exerciseInfo.Steps.SingleOrDefault(step => step.Index == request.StepIndex);
		if (step is null) return new NotFound();

		step.DisplayOrder = request.DisplayOrder;

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
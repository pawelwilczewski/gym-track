using Application.ExerciseInfo.Step.Dtos;
using Application.Persistence;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.ExerciseInfo.Step.Queries;

using ResultType = OneOf<Success<GetExerciseInfoStepResponse>, NotFound>;

public sealed record class GetExerciseInfoStepQuery(
	ExerciseInfoId ExerciseInfoId,
	ExerciseInfoStepIndex StepIndex,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class GetExerciseInfoStepHandler
	: IRequestHandler<GetExerciseInfoStepQuery, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;

	public GetExerciseInfoStepHandler(IUserDataContextFactory dataContextFactory) =>
		this.dataContextFactory = dataContextFactory;

	public async Task<ResultType> Handle(
		GetExerciseInfoStepQuery request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var exerciseInfo = await dataContext.ExerciseInfos.Readable
			.AsNoTrackingWithIdentityResolution()
			.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == request.StepIndex))
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == request.ExerciseInfoId, cancellationToken);

		if (exerciseInfo is null) return new NotFound();

		var step = exerciseInfo.Steps.SingleOrDefault(step => step.Index == request.StepIndex);
		if (step is null) return new NotFound();

		return new Success<GetExerciseInfoStepResponse>(new GetExerciseInfoStepResponse(
			step.Index.Value,
			step.Description.ToString(),
			step.ImageFile?.ToString(),
			step.DisplayOrder));
	}
}
using Application.Files;
using Application.Persistence;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.ExerciseInfo.Step.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class DeleteExerciseInfoStepCommand(
	ExerciseInfoId ExerciseInfoId,
	int StepIndex,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class DeleteExerciseInfoStepHandler
	: IRequestHandler<DeleteExerciseInfoStepCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;
	private readonly IFileStoragePathProvider fileStoragePathProvider;

	public DeleteExerciseInfoStepHandler(IUserDataContextFactory dataContextFactory, IFileStoragePathProvider fileStoragePathProvider)
	{
		this.dataContextFactory = dataContextFactory;
		this.fileStoragePathProvider = fileStoragePathProvider;
	}

	public async Task<ResultType> Handle(
		DeleteExerciseInfoStepCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var exerciseInfo = await dataContext.ExerciseInfos.Modifiable
			.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == request.StepIndex))
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == request.ExerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return new NotFound();

		var exerciseInfoStep = exerciseInfo.Steps.SingleOrDefault(step => step.Index == request.StepIndex);
		if (exerciseInfoStep is null) return new NotFound();

		await EntityImage.Delete(
				exerciseInfoStep.GetImageBaseName(),
				Paths.EXERCISE_INFO_STEP_IMAGES_DIRECTORY_URL,
				fileStoragePathProvider)
			.ConfigureAwait(false);

		exerciseInfo.RemoveStep(exerciseInfoStep, request.UserId);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
using Application.Files;
using Application.Persistence;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.ExerciseInfo.Step.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class UpdateExerciseInfoStepCommand(
	ExerciseInfoId ExerciseInfoId,
	int StepIndex,
	Description Description,
	bool ReplaceImage,
	NamedFile? Image,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class UpdateExerciseInfoStepHandler
	: IRequestHandler<UpdateExerciseInfoStepCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;
	private readonly IFileStoragePathProvider fileStoragePathProvider;

	public UpdateExerciseInfoStepHandler(IUserDataContextFactory dataContextFactory, IFileStoragePathProvider fileStoragePathProvider)
	{
		this.dataContextFactory = dataContextFactory;
		this.fileStoragePathProvider = fileStoragePathProvider;
	}

	public async Task<ResultType> Handle(
		UpdateExerciseInfoStepCommand request,
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

		var imagePath = request.ReplaceImage
			? await request.Image.SaveOrOverrideImage(
					step.GetImageBaseName(),
					Paths.EXERCISE_INFO_STEP_IMAGES_DIRECTORY_URL,
					fileStoragePathProvider,
					cancellationToken)
				.ConfigureAwait(false)
			: step.ImageFile;

		step.Update(request.Description, imagePath, request.UserId);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
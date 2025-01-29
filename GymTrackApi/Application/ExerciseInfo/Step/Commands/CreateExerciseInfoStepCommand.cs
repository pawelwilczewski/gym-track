using Application.ExerciseInfo.Step.Dtos;
using Application.Files;
using Application.Persistence;
using Domain.Common;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.ExerciseInfo.Step.Commands;

using ResultType = OneOf<Success<GetExerciseInfoStepResponse>, NotFound>;

public sealed record class CreateExerciseInfoStepCommand(
	ExerciseInfoId ExerciseInfoId,
	Description Description,
	NamedFile? Image,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class CreateExerciseInfoStepHandler
	: IRequestHandler<CreateExerciseInfoStepCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;
	private readonly IFileStoragePathProvider fileStoragePathProvider;

	public CreateExerciseInfoStepHandler(IUserDataContextFactory dataContextFactory, IFileStoragePathProvider fileStoragePathProvider)
	{
		this.dataContextFactory = dataContextFactory;
		this.fileStoragePathProvider = fileStoragePathProvider;
	}

	public async Task<ResultType> Handle(
		CreateExerciseInfoStepCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var exerciseInfo = await dataContext.ExerciseInfos.Modifiable
			.Include(e => e.Steps)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == request.ExerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return new NotFound();

		var index = exerciseInfo.Steps.GetNextIndex();
		var displayOrder = exerciseInfo.Steps.GetNextDisplayOrder();
		var step = new Domain.Models.ExerciseInfo.ExerciseInfo.Step(
			request.ExerciseInfoId,
			index,
			request.Description,
			null,
			displayOrder);

		exerciseInfo.Steps.Add(step);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		var imagePath = await request.Image.SaveOrOverrideImage(
				step.GetImageBaseName(),
				Paths.EXERCISE_INFO_STEP_IMAGES_DIRECTORY_URL,
				fileStoragePathProvider,
				cancellationToken)
			.ConfigureAwait(false);

		step.Update(request.Description, imagePath, request.UserId);

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<GetExerciseInfoStepResponse>(new GetExerciseInfoStepResponse(
			step.Index,
			step.Description.ToString(),
			step.ImageFile?.ToString(),
			step.DisplayOrder));
	}
}
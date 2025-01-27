using Application.Files;
using Application.Persistence;
using Domain.Common.ValueObjects;
using Domain.Models;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.ExerciseInfo.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class UpdateExerciseInfoCommand(
	Id<Domain.Models.ExerciseInfo.ExerciseInfo> ExerciseInfoId,
	Name Name,
	Description Description,
	bool ReplaceThumbnailImage,
	NamedFile? ThumbnailImage,
	ExerciseMetricType AllowedMetricTypes,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class UpdateExerciseInfoHandler
	: IRequestHandler<UpdateExerciseInfoCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;
	private readonly IFileStoragePathProvider fileStoragePathProvider;

	public UpdateExerciseInfoHandler(IUserDataContextFactory dataContextFactory, IFileStoragePathProvider fileStoragePathProvider)
	{
		this.dataContextFactory = dataContextFactory;
		this.fileStoragePathProvider = fileStoragePathProvider;
	}

	public async Task<ResultType> Handle(
		UpdateExerciseInfoCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var exerciseInfo = await dataContext.ExerciseInfos.Modifiable
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == request.ExerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return new NotFound();

		var path = request.ReplaceThumbnailImage
			? await request.ThumbnailImage.SaveOrOverrideImage(
					exerciseInfo.GetThumbnailImageBaseName(),
					Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY_URL,
					fileStoragePathProvider,
					cancellationToken)
				.ConfigureAwait(false)
			: exerciseInfo.ThumbnailImage;

		exerciseInfo.Update(request.Name, request.Description, path, request.AllowedMetricTypes, request.UserId);

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success();
	}
}
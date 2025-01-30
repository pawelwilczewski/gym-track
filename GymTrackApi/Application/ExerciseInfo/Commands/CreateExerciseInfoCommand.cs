using Application.ExerciseInfo.Dtos;
using Application.Files;
using Application.Persistence;
using Domain.Common.ValueObjects;
using Domain.Models.ExerciseInfo;
using MediatR;
using OneOf.Types;

namespace Application.ExerciseInfo.Commands;

using ResultType = Success<GetExerciseInfoResponse>;

public sealed record class CreateExerciseInfoCommand(
	Name Name,
	Description Description,
	NamedFile? ThumbnailImage,
	ExerciseMetricType AllowedMetricTypes,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class CreateExerciseInfoHandler
	: IRequestHandler<CreateExerciseInfoCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;
	private readonly IFileStoragePathProvider fileStoragePathProvider;

	public CreateExerciseInfoHandler(IUserDataContextFactory dataContextFactory, IFileStoragePathProvider fileStoragePathProvider)
	{
		this.dataContextFactory = dataContextFactory;
		this.fileStoragePathProvider = fileStoragePathProvider;
	}

	public async Task<ResultType> Handle(
		CreateExerciseInfoCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var exerciseInfo = Domain.Models.ExerciseInfo.ExerciseInfo.CreateForUser(
			request.Name,
			null,
			request.Description,
			request.AllowedMetricTypes,
			request.UserId,
			ExerciseInfoId.New());

		var thumbnailImagePath = await request.ThumbnailImage.SaveOrOverrideImage(
				exerciseInfo.GetThumbnailImageBaseName(),
				Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY_URL,
				fileStoragePathProvider,
				cancellationToken)
			.ConfigureAwait(false);

		exerciseInfo.UpdateThumbnailImage(thumbnailImagePath, request.UserId);

		dataContext.ExerciseInfos.Add(exerciseInfo);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return new Success<GetExerciseInfoResponse>(new GetExerciseInfoResponse(
			exerciseInfo.Id.Value,
			exerciseInfo.Name.ToString(),
			exerciseInfo.Description.ToString(),
			exerciseInfo.AllowedMetricTypes,
			exerciseInfo.ThumbnailImage?.ToString(),
			exerciseInfo.Steps
				.Select(step => new ExerciseInfoStepKey(exerciseInfo.Id.Value, step.Index.Value))
				.ToList()));
	}
}
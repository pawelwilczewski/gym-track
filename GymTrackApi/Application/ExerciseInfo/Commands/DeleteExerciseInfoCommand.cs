using Application.Files;
using Application.Persistence;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Application.ExerciseInfo.Commands;

using ResultType = OneOf<Success, NotFound>;

public sealed record class DeleteExerciseInfoCommand(
	Id<Domain.Models.ExerciseInfo.ExerciseInfo> ExerciseInfoId,
	Guid UserId) : IRequest<ResultType>;

// ReSharper disable once UnusedType.Global
internal sealed class DeleteExerciseInfoHandler
	: IRequestHandler<DeleteExerciseInfoCommand, ResultType>
{
	private readonly IUserDataContextFactory dataContextFactory;
	private readonly IFileStoragePathProvider fileStoragePathProvider;

	public DeleteExerciseInfoHandler(IUserDataContextFactory dataContextFactory, IFileStoragePathProvider fileStoragePathProvider)
	{
		this.dataContextFactory = dataContextFactory;
		this.fileStoragePathProvider = fileStoragePathProvider;
	}

	public async Task<ResultType> Handle(
		DeleteExerciseInfoCommand request,
		CancellationToken cancellationToken)
	{
		var dataContext = dataContextFactory.ForUser(request.UserId);

		var exerciseInfo = await dataContext.ExerciseInfos.Modifiable
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == request.ExerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return new NotFound();

		dataContext.ExerciseInfos.Remove(exerciseInfo);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		await EntityImage.Delete(
				exerciseInfo.GetThumbnailImageBaseName(),
				Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY_URL,
				fileStoragePathProvider)
			.ConfigureAwait(false);

		return new Success();
	}
}
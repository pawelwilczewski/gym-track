using Api.Common;
using Api.Files;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos.Steps;

internal sealed class EditExerciseInfoStepImage : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromRoute] int index,
		IFormFile? image,
		[FromServices] IDataContext dataContext,
		[FromServices] IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken)
	{
		var id = new Id<ExerciseInfo>(exerciseInfoId);
		var exerciseInfo = await dataContext.ExerciseInfos.Include(exerciseInfo => exerciseInfo.Users)
			.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == index))
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == id, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		var exerciseInfoStep = exerciseInfo.Steps.SingleOrDefault();
		if (exerciseInfoStep is null) return TypedResults.NotFound("Exercise info step not found.");

		if (image is not null)
		{
			string? localPath = null;
			if (exerciseInfoStep.ImageFile is null)
			{
				var urlPath = $"{Paths.EXERCISE_STEP_INFO_IMAGES_DIRECTORY}/{exerciseInfoId}_{index}{Path.GetExtension(image.FileName)}";
				if (!FilePath.TryCreate(urlPath, out var successfulPath, out var error))
				{
					return error.ToValidationProblem("Image File Path");
				}

				localPath = urlPath.UrlToLocalPath(fileStoragePathProvider);
				await image.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);

				exerciseInfoStep.ImageFile = successfulPath;
			}

			localPath ??= exerciseInfoStep.ImageFile.ToString().UrlToLocalPath(fileStoragePathProvider);
			await image.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);
		}
		else
		{
			var url = exerciseInfoStep.ImageFile;
			if (url is not null)
			{
				File.Delete(url.UrlToLocalPath(fileStoragePathProvider).ToString());
			}

			exerciseInfoStep.ImageFile = null;
		}

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("{index:int}/image", Handler)
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development

		return builder;
	}
}
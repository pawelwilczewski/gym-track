using Api.Common;
using Api.Files;
using Application.Persistence;
using Domain.Common;
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
		[FromForm] IFormFile? image,
		[FromServices] IDataContext dataContext,
		IWebHostEnvironment environment,
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
			if (exerciseInfoStep.ImageFile.Reduce(null) is null)
			{
				var urlPath = $"{Paths.EXERCISE_STEP_INFO_IMAGES_DIRECTORY}/{exerciseInfoId}_{index}{Path.GetExtension(image.FileName)}";
				if (!FilePath.TryCreate(urlPath, out var successfulPath, out var invalidPath))
				{
					return invalidPath.ToValidationProblem("Image File Path");
				}

				localPath = Paths.UrlToLocal(urlPath, environment);
				await image.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);

				exerciseInfoStep.ImageFile = Option<FilePath>.Some(successfulPath);
			}

			localPath ??= Paths.UrlToLocal(exerciseInfoStep.ImageFile.Reduce(null)!.ToString(), environment);
			await image.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);
		}
		else
		{
			var url = exerciseInfoStep.ImageFile.Reduce(null);
			if (url is not null)
			{
				File.Delete(Paths.UrlToLocal(url, environment).ToString());
			}

			exerciseInfoStep.ImageFile = Option<FilePath>.None();
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
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

internal sealed class CreateExerciseInfoStep : IEndpoint
{
	public static async Task<Results<Created, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromForm] int index,
		[FromForm] string description,
		[FromForm] IFormFile? image,
		[FromServices] IDataContext dataContext,
		[FromServices] IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken)
	{
		if (index < 0) return ValidationErrors.NegativeIndex();

		if (!Description.TryCreate(description, out var exerciseInfoStepDescription, out var error))
		{
			return error.ToValidationProblem("Description");
		}

		var id = new Id<ExerciseInfo>(exerciseInfoId);
		var exerciseInfo = await dataContext.ExerciseInfos.Include(exerciseInfo => exerciseInfo.Users)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == id, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		Option<FilePath> path;
		if (image is not null)
		{
			var urlPath = $"{Paths.EXERCISE_STEP_INFO_IMAGES_DIRECTORY}/{exerciseInfoId}_{index}{Path.GetExtension(image.FileName)}";
			if (!FilePath.TryCreate(urlPath, out var successfulPath, out error))
			{
				return error.ToValidationProblem("Image File Path");
			}

			var localPath = urlPath.UrlToLocalPath(fileStoragePathProvider);
			Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
			await image.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);

			path = Option<FilePath>.Some(successfulPath);
		}
		else
		{
			path = Option<FilePath>.None();
		}

		var exerciseInfoStep = new ExerciseInfo.Step(id, index, exerciseInfoStepDescription, path);

		exerciseInfo.Steps.Add(exerciseInfoStep);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.Created();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler)
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development
		return builder;
	}
}
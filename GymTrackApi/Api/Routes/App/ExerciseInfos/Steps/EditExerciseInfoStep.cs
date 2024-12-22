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

internal sealed class EditExerciseInfoStep : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromRoute] int index,
		[FromForm] string description,
		[FromForm] bool replaceImage,
		IFormFile? image,
		[FromServices] IDataContext dataContext,
		[FromServices] IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken)
	{
		var id = new Id<ExerciseInfo>(exerciseInfoId);
		var exerciseInfo = await dataContext.ExerciseInfos
			.Include(exerciseInfo => exerciseInfo.Users)
			.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == index))
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == id, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		var step = exerciseInfo.Steps.SingleOrDefault();
		if (step is null) return TypedResults.NotFound("Step not found.");

		if (!step.Description.TrySet(description, out var error))
		{
			return error.ToValidationProblem("Description");
		}

		if (replaceImage)
		{
			step.ImageFile = await image.SaveOrOverrideImage(
					step.GetImageBaseName(),
					Paths.EXERCISE_INFO_STEP_IMAGES_DIRECTORY_URL,
					fileStoragePathProvider,
					cancellationToken)
				.ConfigureAwait(false);
		}

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{index:int}", Handler)
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development
		return builder;
	}
}
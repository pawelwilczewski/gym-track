using Api.Common;
using Api.Files;
using Application.Persistence;
using Domain.Common;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.ExerciseInfo;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos.Steps;

internal sealed class CreateExerciseInfoStep : IEndpoint
{
	public static async Task<Results<Created, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromForm] string description,
		IFormFile? image,
		[FromServices] IDataContext dataContext,
		[FromServices] IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken)
	{
		if (!Description.TryCreate(description, out var exerciseInfoStepDescription, out var error))
		{
			return error.ToValidationProblem("Description");
		}

		var id = new Id<ExerciseInfo>(exerciseInfoId);
		var exerciseInfo = await dataContext.ExerciseInfos
			.Include(e => e.Steps)
			.Include(exerciseInfo => exerciseInfo.Users)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == id, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		var index = exerciseInfo.Steps.GetNextIndex();
		var displayOrder = exerciseInfo.Steps.GetNextDisplayOrder();
		var exerciseInfoStep = new ExerciseInfo.Step(id, index, exerciseInfoStepDescription, null, displayOrder);

		var imagePath = await image.SaveOrOverrideImage(
				exerciseInfoStep.GetImageBaseName(),
				Paths.EXERCISE_INFO_STEP_IMAGES_DIRECTORY_URL,
				fileStoragePathProvider,
				cancellationToken)
			.ConfigureAwait(false);
		exerciseInfoStep.ImageFile = imagePath;

		exerciseInfo.Steps.Add(exerciseInfoStep);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.Created($"{httpContext.Request.Path}/{exerciseInfoStep.Index}");
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler);
		return builder;
	}
}
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

internal sealed class DeleteExerciseInfoStep : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromRoute] int stepIndex,
		[FromServices] IDataContext dataContext,
		[FromServices] IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken)
	{
		var id = new Id<ExerciseInfo>(exerciseInfoId);
		var exerciseInfoStep = await dataContext.ExerciseInfoSteps
			.Include(exerciseInfoStep => exerciseInfoStep.ExerciseInfo)
			.ThenInclude(exerciseInfo => exerciseInfo.Users)
			.FirstOrDefaultAsync(exerciseInfoStep => exerciseInfoStep.ExerciseInfoId == id
				&& exerciseInfoStep.Index == stepIndex, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfoStep is null) return TypedResults.NotFound("Step not found");

		var exerciseInfo = exerciseInfoStep.ExerciseInfo;
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		await EntityImage.Delete(
				exerciseInfoStep.GetImageBaseName(),
				Paths.EXERCISE_INFO_STEP_IMAGES_DIRECTORY_URL,
				fileStoragePathProvider)
			.ConfigureAwait(false);

		exerciseInfo.Steps.Remove(exerciseInfoStep);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{stepIndex:int}", Handler);
		return builder;
	}
}
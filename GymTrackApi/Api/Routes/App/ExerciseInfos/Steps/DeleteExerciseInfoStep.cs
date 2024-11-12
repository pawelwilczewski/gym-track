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
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromRoute] int index,
		[FromServices] IDataContext dataContext,
		[FromServices] IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken)
	{
		var id = new Id<ExerciseInfo>(exerciseInfoId);
		var exerciseInfoStep = await dataContext.ExerciseInfoSteps
			.Include(exerciseInfoStep => exerciseInfoStep.ExerciseInfo)
			.ThenInclude(exerciseInfo => exerciseInfo.Users)
			.FirstOrDefaultAsync(exerciseInfoStep => exerciseInfoStep.ExerciseInfoId == id
				&& exerciseInfoStep.Index == index, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfoStep is null) return TypedResults.NotFound("Step not found");

		var exerciseInfo = exerciseInfoStep.ExerciseInfo;
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		if (exerciseInfoStep.ImageFile is not null)
		{
			File.Delete(exerciseInfoStep.ImageFile.ToString().UrlToLocalPath(fileStoragePathProvider));
		}

		exerciseInfo.Steps.Remove(exerciseInfoStep);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{index:int}", Handler);
		return builder;
	}
}
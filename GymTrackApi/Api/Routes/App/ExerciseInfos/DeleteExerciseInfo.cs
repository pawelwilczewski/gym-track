using Api.Files;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos;

internal sealed class DeleteExerciseInfo : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid id,
		[FromServices] IDataContext dataContext,
		[FromServices] IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken)
	{
		var exerciseInfoId = new Id<ExerciseInfo>(id);
		var exerciseInfo = await dataContext.ExerciseInfos.Include(exerciseInfo => exerciseInfo.Users)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == exerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		dataContext.ExerciseInfos.Remove(exerciseInfo);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		await ThumbnailImage.Delete(exerciseInfoId, fileStoragePathProvider).ConfigureAwait(false);

		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{id:guid}", Handler);
		return builder;
	}
}
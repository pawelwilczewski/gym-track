using Api.Files;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos;

internal sealed class EditExerciseInfoThumbnail : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid id,
		[FromForm] IFormFile thumbnailImage,
		[FromServices] IDataContext dataContext,
		IWebHostEnvironment environment,
		CancellationToken cancellationToken)
	{
		var exerciseInfoId = new Id<ExerciseInfo>(id);
		var exerciseInfo = await dataContext.ExerciseInfos.Include(exerciseInfo => exerciseInfo.Users)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == exerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		var urlPath = $"{Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY}/{id}{Path.GetExtension(thumbnailImage.FileName)}";
		var localPath = Path.Combine(environment.WebRootPath, urlPath.Replace('/', Path.DirectorySeparatorChar));
		await thumbnailImage.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);

		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("{id:guid}/thumbnail", Handler)
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development
		return builder;
	}
}
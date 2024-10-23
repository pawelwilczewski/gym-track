using Api.Files;
using Application.Persistence;
using Domain.Common;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfoEndpoints.Step;

internal sealed class CreateExerciseInfoStep : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/", async Task<Results<Ok, BadRequest<string>, NotFound>> (
				HttpContext httpContext,
				[FromRoute] Guid exerciseInfoId,
				[FromForm] int index,
				[FromForm] string description,
				[FromForm] IFormFile? image,
				[FromServices] IDataContext dataContext,
				IWebHostEnvironment environment,
				CancellationToken cancellationToken) =>
			{
				if (!Description.TryCreate(description, out var exerciseInfoStepDescription, out var invalidDescription))
				{
					return TypedResults.BadRequest(invalidDescription.Error);
				}

				var id = new Id<ExerciseInfo>(exerciseInfoId);
				var exerciseInfo = await dataContext.ExerciseInfos
					.Include(exerciseInfo => exerciseInfo.Users)
					.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == id, cancellationToken)
					.ConfigureAwait(false);

				if (exerciseInfo is null || !httpContext.User.CanModifyOrDelete(exerciseInfo.Users, out _)) // TODO Pawel: this is inconsistent to other usages of CanModifyOrDelete - should we return little info with NotFound or a more specific code instead?
				{
					return TypedResults.NotFound();
				}

				Option<FilePath> path;
				if (image is not null)
				{
					var urlPath = $"{Paths.EXERCISE_STEP_INFO_IMAGES_DIRECTORY}/{exerciseInfoId}_{index}{Path.GetExtension(image.FileName)}";
					if (!FilePath.TryCreate(urlPath, out var successfulPath, out var invalidPath))
					{
						return TypedResults.BadRequest(invalidPath.Error);
					}

					var localPath = Paths.UrlToLocal(urlPath, environment);
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

				return TypedResults.Ok();
			})
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development

		return builder;
	}
}
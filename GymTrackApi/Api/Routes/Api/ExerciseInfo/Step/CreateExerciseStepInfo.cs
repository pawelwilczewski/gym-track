using Api.Files;
using Application.Persistence;
using Domain.Common;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Api.ExerciseInfo.Step;

internal sealed class CreateExerciseStepInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/create", async Task<Results<Ok, BadRequest<string>, NotFound>> (
				HttpContext httpContext,
				[FromForm] Guid id,
				[FromForm] int index,
				[FromForm] string description,
				[FromForm] IFormFile? image,
				[FromServices] IDataContext dataContext,
				IWebHostEnvironment environment,
				CancellationToken cancellationToken) =>
			{
				if (!Description.TryCreate(description, out var exerciseStepInfoDescription, out var invalidDescription))
				{
					return TypedResults.BadRequest(invalidDescription.Error);
				}

				if (index < 0)
				{
					return TypedResults.BadRequest("Index must be greater than or equal to 0.");
				}

				var exerciseInfoId = new Id<Domain.Models.Workout.ExerciseInfo>(id);
				var exerciseInfo = await dataContext.ExerciseInfos
					.FindAsync([exerciseInfoId], cancellationToken)
					.ConfigureAwait(false);

				if (exerciseInfo is null
					|| httpContext.User.CanModifyOrDelete(exerciseInfo.Users) is not CanModifyResult.Ok)
				{
					return TypedResults.NotFound();
				}

				Option<FilePath> path;
				if (image is not null)
				{
					var urlPath = $"{Paths.EXERCISE_STEP_INFO_IMAGES_DIRECTORY}/{id}_{index}{Path.GetExtension(image.FileName)}";
					if (!FilePath.TryCreate(urlPath, out var successfulPath, out var invalidPath))
					{
						return TypedResults.BadRequest(invalidPath.Error);
					}

					var localPath = Path.Combine(
						environment.WebRootPath,
						urlPath.Replace('/', Path.DirectorySeparatorChar));

					await image.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);

					path = Option<FilePath>.Some(successfulPath);
				}
				else
				{
					path = Option<FilePath>.None();
				}

				var exerciseStepInfo = new ExerciseStepInfo(exerciseInfoId, index, exerciseStepInfoDescription, path);

				exerciseInfo.Steps.Add(exerciseStepInfo);
				await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

				return TypedResults.Ok();
			})
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development

		return builder;
	}
}
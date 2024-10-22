using Api.Files;
using Application.Persistence;
using Domain.Common;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Api.ExerciseInfo.Step;

internal sealed class EditExerciseStepInfoImage : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/{index:int}/image", async Task<Results<Ok, BadRequest<string>, NotFound>> (
				HttpContext httpContext,
				[FromRoute] Guid exerciseInfoId,
				[FromRoute] int index,
				[FromForm] IFormFile? image,
				[FromServices] IDataContext dataContext,
				IWebHostEnvironment environment,
				CancellationToken cancellationToken) =>
			{
				var id = new Id<Domain.Models.Workout.ExerciseInfo>(exerciseInfoId);
				var exerciseInfo = await dataContext.ExerciseInfos
					.Include(exerciseInfo => exerciseInfo.Users)
					.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == index))
					.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == id, cancellationToken)
					.ConfigureAwait(false);

				if (exerciseInfo is null || !httpContext.User.CanModifyOrDelete(exerciseInfo.Users, out _)) // TODO Pawel: this is inconsistent to other usages of CanModifyOrDelete - should we return little info with NotFound or a more specific code instead?
				{
					return TypedResults.NotFound();
				}

				var exerciseStepInfo = exerciseInfo.Steps.SingleOrDefault();
				if (exerciseStepInfo is null) return TypedResults.NotFound();

				string? test = null;
				var a = FilePath.OptionalConverter.ConvertFromProviderTyped(test);

				if (image is not null)
				{
					string? localPath = null;
					if (exerciseStepInfo.ImageFile.Reduce(null) is null)
					{
						var urlPath = $"{Paths.EXERCISE_STEP_INFO_IMAGES_DIRECTORY}/{exerciseInfoId}_{index}{Path.GetExtension(image.FileName)}";
						if (!FilePath.TryCreate(urlPath, out var successfulPath, out var invalidPath))
						{
							return TypedResults.BadRequest(invalidPath.Error);
						}

						localPath = Paths.UrlToLocal(urlPath, environment);
						await image.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);

						exerciseStepInfo.ImageFile = Option<FilePath>.Some(successfulPath);
					}

					localPath ??= Paths.UrlToLocal(exerciseStepInfo.ImageFile.Reduce(null)!.ToString(), environment);
					await image.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					var url = exerciseStepInfo.ImageFile.Reduce(null);
					if (url is not null)
					{
						File.Delete(Paths.UrlToLocal(url, environment).ToString());
					}

					exerciseStepInfo.ImageFile = Option<FilePath>.None();
				}

				await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

				return TypedResults.Ok();
			})
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development

		return builder;
	}
}
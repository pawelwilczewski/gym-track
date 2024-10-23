using Api.Files;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfoEndpointsEndpoints;

internal sealed class CreateExerciseInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/", async Task<Results<Ok, BadRequest<string>>> (
				HttpContext httpContext,
				[FromForm] string name,
				[FromForm] string description,
				[FromForm] ExerciseMetricType allowedMetricTypes,
				[FromForm] IFormFile thumbnailImage,
				[FromServices] IDataContext dataContext,
				IWebHostEnvironment environment,
				CancellationToken cancellationToken) =>
			{
				if (!Name.TryCreate(name, out var exerciseInfoName, out var invalidName))
				{
					return TypedResults.BadRequest(invalidName.Error);
				}

				if (!Description.TryCreate(description, out var exerciseInfoDescription, out var invalidDescription))
				{
					return TypedResults.BadRequest(invalidDescription.Error);
				}

				var id = Id<ExerciseInfo>.New();

				var urlPath = $"{Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY}/{id}{Path.GetExtension(thumbnailImage.FileName)}";
				if (!FilePath.TryCreate(urlPath, out var path, out var invalidPath))
				{
					return TypedResults.BadRequest(invalidPath.Error);
				}

				var localPath = Path.Combine(environment.WebRootPath, urlPath.Replace('/', Path.DirectorySeparatorChar));
				await thumbnailImage.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);

				var exerciseInfo = httpContext.User.IsInRole(Role.ADMINISTRATOR)
					? ExerciseInfo.CreateForEveryone(
						exerciseInfoName, path, exerciseInfoDescription, allowedMetricTypes, id)
					: ExerciseInfo.CreateForUser(
						exerciseInfoName, path, exerciseInfoDescription, allowedMetricTypes, httpContext.User, id);

				dataContext.ExerciseInfos.Add(exerciseInfo);
				await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

				return TypedResults.Ok();
			})
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development

		return builder;
	}
}
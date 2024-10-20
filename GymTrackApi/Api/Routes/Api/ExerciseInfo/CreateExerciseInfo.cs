using Api.Files;
using Application.Persistence;
using Domain.Common;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Domain.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Api.ExerciseInfo;

internal sealed class CreateExerciseInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/create", async Task<Results<Ok, BadRequest<string>>> (
				HttpContext httpContext,
				[FromForm] string name,
				[FromForm] string description,
				[FromForm] ExerciseMetricType allowedMetricTypes,
				[FromForm] IFormFile thumbnailImage,
				[FromServices] IDataContext dataContext,
				IWebHostEnvironment environment,
				CancellationToken cancellationToken) =>
			{
				if (Name.TryCreate(name, out var exerciseInfoName)
					is TextValidationResult.Invalid invalidName)
				{
					return TypedResults.BadRequest(invalidName.Error);
				}

				if (Description.TryCreate(description, out var exerciseInfoDescription)
					is TextValidationResult.Invalid invalidDescription)
				{
					return TypedResults.BadRequest(invalidDescription.Error);
				}

				var exerciseInfo = httpContext.User.IsInRole(Role.ADMINISTRATOR)
					? Domain.Models.Workout.ExerciseInfo.CreateForEveryone(
						exerciseInfoName, new FilePath(), exerciseInfoDescription, allowedMetricTypes)
					: Domain.Models.Workout.ExerciseInfo.CreateForUser(
						exerciseInfoName, new FilePath(), exerciseInfoDescription, allowedMetricTypes, httpContext.User);

				var urlPath = $"{Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY}/{exerciseInfo.Id}{Path.GetExtension(thumbnailImage.FileName)}";
				var localPath = Path.Combine(environment.WebRootPath, urlPath.Replace('/', Path.DirectorySeparatorChar));
				await using (var stream = thumbnailImage.OpenReadStream())
				{
					var outputStream = File.Create(localPath);
					await stream.CopyToAsync(outputStream, cancellationToken);
					await outputStream.DisposeAsync();
				}

				exerciseInfo.ThumbnailImage = new FilePath(urlPath);

				dataContext.ExerciseInfos.Add(exerciseInfo);
				await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

				return TypedResults.Ok();
			})
			.DisableAntiforgery() // TODO Pawel: enable anti forgery outside of development
			.RequireAuthorization();

		return builder;
	}
}
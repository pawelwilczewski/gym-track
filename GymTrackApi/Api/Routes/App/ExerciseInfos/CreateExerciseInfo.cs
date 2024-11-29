using Api.Common;
using Api.Files;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfos;

internal sealed class CreateExerciseInfo : IEndpoint
{
	public static async Task<Results<Created, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromForm] string name,
		[FromForm] string description,
		[FromForm] ExerciseMetricType allowedMetricTypes,
		IFormFile? thumbnailImage,
		[FromServices] IDataContext dataContext,
		[FromServices] IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken)
	{
		if (!Name.TryCreate(name, out var exerciseInfoName, out var error))
		{
			return error.ToValidationProblem(nameof(name));
		}

		if (!Description.TryCreate(description, out var exerciseInfoDescription, out error))
		{
			return error.ToValidationProblem(nameof(description));
		}

		var id = Id<ExerciseInfo>.New();

		FilePath? path = null;
		if (thumbnailImage is not null)
		{
			var urlPath = $"{Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY}/{id}{Path.GetExtension(thumbnailImage.FileName)}";
			if (!FilePath.TryCreate(urlPath, out path, out error))
			{
				return error.ToValidationProblem(nameof(thumbnailImage));
			}

			var localPath = Path.Combine(fileStoragePathProvider.RootPath, urlPath.Replace('/', Path.DirectorySeparatorChar));
			Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
			await thumbnailImage.SaveToFile(localPath, cancellationToken).ConfigureAwait(false);
		}

		var exerciseInfo = httpContext.User.IsInRole(Role.ADMINISTRATOR)
			? ExerciseInfo.CreateForEveryone(exerciseInfoName, path, exerciseInfoDescription, allowedMetricTypes, id)
			: ExerciseInfo.CreateForUser(exerciseInfoName, path, exerciseInfoDescription, allowedMetricTypes, httpContext.User, id);

		dataContext.ExerciseInfos.Add(exerciseInfo);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.Created($"{httpContext.Request.Path}/{exerciseInfo.Id}");
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler)
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development
		return builder;
	}
}
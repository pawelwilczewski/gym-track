using Api.Common;
using Api.Files;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos;

internal sealed class EditExerciseInfo : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid id,
		[FromForm] string name,
		[FromForm] string description,
		[FromForm] ExerciseMetricType allowedMetricTypes,
		[FromForm] bool replaceThumbnailImage,
		IFormFile? thumbnailImage,
		[FromServices] IDataContext dataContext,
		[FromServices] IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken)
	{
		if ((allowedMetricTypes & ExerciseMetricType.All) == 0)
		{
			return TypedResults.ValidationProblem(new Dictionary<string, string[]>
			{
				{ "AllowedMetricTypes", ["At least one metric type must be selected."] }
			});
		}

		var exerciseInfoId = new Id<ExerciseInfo>(id);
		var exerciseInfo = await dataContext.ExerciseInfos.Include(exerciseInfo => exerciseInfo.Users)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == exerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		if (!exerciseInfo.Name.TrySet(name, out var error))
		{
			return error.ToValidationProblem(nameof(ExerciseInfo.Name));
		}

		if (!exerciseInfo.Description.TrySet(description, out error))
		{
			return error.ToValidationProblem(nameof(ExerciseInfo.Description));
		}

		exerciseInfo.AllowedMetricTypes = allowedMetricTypes;

		if (replaceThumbnailImage)
		{
			var localThumbnailsDirectory = Path.Combine(
				fileStoragePathProvider.RootPath,
				Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY.Replace('/', Path.DirectorySeparatorChar));

			var matchingFiles = Directory.EnumerateFiles(
					localThumbnailsDirectory,
					$"{id}.*", SearchOption.TopDirectoryOnly)
				.ToList();

			if (matchingFiles.Count > 1)
			{
				await Console.Error.WriteLineAsync("More than one thumbnail image found. This should never happen.")
					.ConfigureAwait(false);
			}

			var currentThumbnailPath = matchingFiles switch
			{
				[]              => null,
				[{ } single]    => single,
				[{ } first, ..] => first
			};

			if (currentThumbnailPath is not null)
			{
				try
				{
					File.Delete(currentThumbnailPath);
				}
				catch (IOException ioException)
				{
					await Console.Error.WriteLineAsync($"Could not delete thumbnail file: ${ioException.Message}");
				}
			}

			if (thumbnailImage is null)
			{
				exerciseInfo.ThumbnailImage = null;
			}
			else
			{
				var thumbnailPathString =
					$"{localThumbnailsDirectory}/{id}{Path.GetExtension(thumbnailImage.FileName)}"
						.Replace('/', Path.DirectorySeparatorChar);

				var thumbnailDbPath = $"{Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY}/{id}{Path.GetExtension(thumbnailImage.FileName)}"
					.Replace('/', Path.DirectorySeparatorChar);
				if (!FilePath.TryCreate(thumbnailDbPath,
					out var finalThumbnailPath, out error))
				{
					return error.ToValidationProblem(nameof(ExerciseInfo.ThumbnailImage));
				}

				await thumbnailImage.SaveToFile(thumbnailPathString, cancellationToken)
					.ConfigureAwait(false);

				exerciseInfo.ThumbnailImage = finalThumbnailPath;
			}
		}

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{id:guid}", Handler)
			.DisableAntiforgery(); // TODO Pawel: enable anti forgery outside of development
		return builder;
	}
}
using System.Diagnostics;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Workout;

namespace Api.Files;

internal static class ThumbnailImage
{
	private static string GetLocalThumbnailsDirectory(IFileStoragePathProvider fileStoragePathProvider) =>
		Path.Combine(
			fileStoragePathProvider.RootPath,
			Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY.Replace('/', Path.DirectorySeparatorChar));

	public static async Task<FilePath?> SaveOrOverrideAsThumbnailImage(
		this IFormFile? thumbnailFile,
		Id<ExerciseInfo> id,
		IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken = default)
	{
		await Delete(id, fileStoragePathProvider);

		if (thumbnailFile is null)
		{
			return null;
		}

		// _GUID to fix image caching issues (same url, image wouldn't refresh on page)
		var thumbnailFileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(thumbnailFile.FileName)}";

		var localThumbnailPathString =
			$"{GetLocalThumbnailsDirectory(fileStoragePathProvider)}/{thumbnailFileName}"
				.Replace('/', Path.DirectorySeparatorChar);

		await thumbnailFile.SaveToFile(localThumbnailPathString, cancellationToken)
			.ConfigureAwait(false);

		var thumbnailDbPath = $"{Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY}/{thumbnailFileName}"
			.Replace('/', Path.DirectorySeparatorChar);

		if (!FilePath.TryCreate(thumbnailDbPath,
			out var finalThumbnailPath, out var error))
		{
			throw new UnreachableException(
				$"Couldn't create thumbnail file, this may be due to invalid config path: "
				+ $"{Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY}\nError: {error}");
		}

		return finalThumbnailPath;
	}

	public static async Task Delete(
		Id<ExerciseInfo> id,
		IFileStoragePathProvider fileStoragePathProvider)
	{
		var matchingFiles = Directory.EnumerateFiles(
			GetLocalThumbnailsDirectory(fileStoragePathProvider),
			$"{id}*.*", SearchOption.TopDirectoryOnly);

		foreach (var path in matchingFiles)
		{
			try
			{
				File.Delete(path);
			}
			catch (IOException ioException)
			{
				await Console.Error.WriteLineAsync($"Could not delete thumbnail file: ${ioException.Message}");
			}
		}
	}
}
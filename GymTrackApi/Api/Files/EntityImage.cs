using System.Diagnostics;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Workout;

namespace Api.Files;

internal static class EntityImage
{
	public static async Task<FilePath?> SaveOrOverrideAsThumbnailImage(
		this IFormFile? thumbnailFile,
		Id<ExerciseInfo> id,
		IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken = default)
	{
		await Delete(id, fileStoragePathProvider);

		if (thumbnailFile is null) return null;

		// _GUID to fix image caching issues (same url, image wouldn't refresh on page)
		var fileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(thumbnailFile.FileName)}";

		var localPath = Path.Combine(
			Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY.UrlToLocalPath(fileStoragePathProvider),
			fileName);

		if (!FilePath.TryCreate(Path.Combine(Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY, fileName),
			out var dbPath, out var error))
		{
			throw new UnreachableException(
				$"Couldn't create thumbnail file, this may be due to invalid config path: "
				+ $"{Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY}\nError: {error}");
		}

		await thumbnailFile.SaveToFile(localPath, cancellationToken)
			.ConfigureAwait(false);

		return dbPath;
	}

	public static async Task Delete(
		Id<ExerciseInfo> id,
		IFileStoragePathProvider fileStoragePathProvider)
	{
		var matchingFiles = Directory.EnumerateFiles(
			Paths.EXERCISE_INFO_THUMBNAILS_DIRECTORY.UrlToLocalPath(fileStoragePathProvider),
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
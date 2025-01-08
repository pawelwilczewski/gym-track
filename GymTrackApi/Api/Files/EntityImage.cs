using System.Diagnostics;
using Application.Persistence;
using Domain.Models;

namespace Api.Files;

internal static class EntityImage
{
	public static async Task<FilePath?> SaveOrOverrideImage(
		this IFormFile? imageFile,
		string baseName,
		string directoryUrl,
		IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken = default)
	{
		await Delete(baseName, directoryUrl, fileStoragePathProvider);

		if (imageFile is null) return null;

		// _GUID to fix image caching issues (same url, image wouldn't refresh on page)
		var fileName = $"{baseName}_{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

		if (!FilePath.TryCreate(Path.Combine(directoryUrl, fileName),
			out var dbPath, out var error))
		{
			throw new UnreachableException(
				$"Couldn't create image file, this may be due to invalid config path: "
				+ $"{directoryUrl}\nError: {error}");
		}

		var localDirectoryPath = directoryUrl.UrlToLocalPath(fileStoragePathProvider);
		Directory.CreateDirectory(localDirectoryPath);

		var localFilePath = Path.Combine(localDirectoryPath, fileName);

		await imageFile.SaveToFile(localFilePath, cancellationToken)
			.ConfigureAwait(false);

		return dbPath;
	}

	public static async Task Delete(
		string baseName,
		string directoryUrl,
		IFileStoragePathProvider fileStoragePathProvider)
	{
		var localDirectory = directoryUrl.UrlToLocalPath(fileStoragePathProvider);
		if (!Directory.Exists(localDirectory)) return;

		var matchingFiles = Directory.EnumerateFiles(
			localDirectory,
			$"{baseName}*.*", SearchOption.TopDirectoryOnly);

		foreach (var path in matchingFiles)
		{
			try
			{
				File.Delete(path);
			}
			catch (IOException ioException)
			{
				await Console.Error.WriteLineAsync($"Could not delete image file: ${ioException.Message}");
			}
		}
	}
}
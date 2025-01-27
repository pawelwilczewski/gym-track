using System.Diagnostics;
using Application.Persistence;
using Domain.Common.ValueObjects;

namespace Application.Files;

internal static class EntityImage
{
	public static async Task<FilePath?> SaveOrOverrideImage(
		this NamedFile? imageFile,
		string baseName,
		string directoryUrl,
		IFileStoragePathProvider fileStoragePathProvider,
		CancellationToken cancellationToken = default)
	{
		await Delete(baseName, directoryUrl, fileStoragePathProvider);

		if (imageFile is null) return null;

		// _GUID to fix image caching issues (same url, image wouldn't refresh on page)
		var fileName = $"{baseName}_{Guid.NewGuid()}{Path.GetExtension(imageFile.Value.FileName)}";

		var result = FilePath.TryFrom(Path.Combine(directoryUrl, fileName));
		if (!result.IsSuccess)
		{
			throw new UnreachableException(
				$"Couldn't create image file, this may be due to invalid config path: "
				+ $"{directoryUrl}\nError: {result.Error.ErrorMessage}");
		}

		var localDirectoryPath = directoryUrl.UrlToLocalPath(fileStoragePathProvider);
		Directory.CreateDirectory(localDirectoryPath);

		var localFilePath = Path.Combine(localDirectoryPath, fileName);

		await imageFile.Value.SaveToFile(localFilePath, cancellationToken)
			.ConfigureAwait(false);

		return result.ValueObject;
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
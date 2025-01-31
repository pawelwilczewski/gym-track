using Application.Persistence;
using Domain.Common.ValueObjects;

namespace Application.Files;

internal static class Paths
{
	// TODO Pawel: this should be stored in some config file instead
	public const string EXERCISE_INFO_THUMBNAILS_DIRECTORY_URL = "images/exercise-info/thumbnails";
	public const string EXERCISE_INFO_STEP_IMAGES_DIRECTORY_URL = "images/exercise-info/step";

	public static FilePath UrlToLocalPath(this FilePath url, IFileStoragePathProvider fileStoragePathProvider)
	{
		var result = FilePath.TryFrom(UrlToLocalPath(url.ToString(), fileStoragePathProvider));
		return result.IsSuccess
			? result.ValueObject
			: throw new Exception($"Can't create local path from url: {url}");
	}

	public static string UrlToLocalPath(this string url, IFileStoragePathProvider fileStoragePathProvider) =>
		Path.Combine(fileStoragePathProvider.RootPath, url.Replace('/', Path.DirectorySeparatorChar));
}
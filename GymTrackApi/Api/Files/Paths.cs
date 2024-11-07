using Application.Persistence;
using Domain.Models;

namespace Api.Files;

internal static class Paths
{
	// TODO Pawel: this should be stored in some config file instead
	public const string EXERCISE_INFO_THUMBNAILS_DIRECTORY = "images/exerciseInfo/thumbnails";
	public const string EXERCISE_STEP_INFO_IMAGES_DIRECTORY = "images/exerciseInfo/step";

	public static FilePath UrlToLocalPath(this FilePath url, IFileStoragePathProvider fileStoragePathProvider) =>
		FilePath.TryCreate(UrlToLocalPath(url.ToString(), fileStoragePathProvider), out var result, out _)
			? result
			: throw new Exception($"Can't create local path from url: {url}");

	public static string UrlToLocalPath(this string url, IFileStoragePathProvider fileStoragePathProvider) =>
		Path.Combine(fileStoragePathProvider.RootPath, url.Replace('/', Path.DirectorySeparatorChar));
}
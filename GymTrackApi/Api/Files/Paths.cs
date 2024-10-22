using Domain.Models;

namespace Api.Files;

internal static class Paths
{
	public const string EXERCISE_INFO_THUMBNAILS_DIRECTORY = "images/exerciseInfo/thumbnails";
	public const string EXERCISE_STEP_INFO_IMAGES_DIRECTORY = "images/exerciseInfo/stepInfo";

	public static FilePath UrlToLocal(FilePath url, IWebHostEnvironment environment) =>
		FilePath.TryCreate(UrlToLocal(url.ToString(), environment), out var result, out _)
			? result
			: throw new Exception($"Can't create local path from url: {url}");

	public static string UrlToLocal(string url, IWebHostEnvironment environment) =>
		Path.Combine(environment.WebRootPath, url.Replace('/', Path.DirectorySeparatorChar));
}
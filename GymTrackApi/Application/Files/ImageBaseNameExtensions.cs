using Domain.Models.ExerciseInfo;

namespace Application.Files;

internal static class ImageBaseNameExtensions
{
	public static string GetThumbnailImageBaseName(this Domain.Models.ExerciseInfo.ExerciseInfo exerciseInfo) =>
		exerciseInfo.Id.ToString();

	public static string GetImageBaseName(this ExerciseInfoStep step) =>
		$"{step.ExerciseInfoId}_{step.Index}";
}
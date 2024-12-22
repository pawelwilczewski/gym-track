using Domain.Models.Workout;

namespace Api.Common;

internal static class ImageBaseNameExtensions
{
	public static string GetThumbnailImageBaseName(this ExerciseInfo exerciseInfo) =>
		exerciseInfo.Id.ToString();

	public static string GetImageBaseName(this ExerciseInfo.Step step) =>
		$"{step.ExerciseInfoId}_{step.Index}";
}
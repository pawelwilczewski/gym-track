using Domain.Common;

namespace Domain.Models.Workout;

public class ExerciseStepInfo
{
	public Id<ExerciseInfo> ExerciseInfoId { get; set; }
	public int Index { get; set; }

	public string? Description;
	public OptionalFilePath ImageFile;
}
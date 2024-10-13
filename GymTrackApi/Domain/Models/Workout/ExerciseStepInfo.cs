using Domain.Common;

namespace Domain.Models.Workout;

public class ExerciseStepInfo
{
	public Id<ExerciseInfo> ExerciseInfoId { get; set; }
	public required int Index { get; set; }

	public required ExerciseInfo ExerciseInfo { get; set; }

	public string? Description { get; set; }
	public OptionalFilePath ImageFile { get; set; }
}
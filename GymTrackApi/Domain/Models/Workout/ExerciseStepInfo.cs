using Domain.Common;

namespace Domain.Models.Workout;

public class ExerciseStepInfo
{
	public Id<ExerciseInfo> ExerciseInfoId { get; private set; }
	public int Index { get; private set; }

	public ExerciseInfo ExerciseInfo { get; private set; } = default!;

	public string? Description { get; private set; }
	public OptionalFilePath ImageFile { get; private set; }

	private ExerciseStepInfo() { }

	public ExerciseStepInfo(Id<ExerciseInfo> exerciseInfoId, int index, string? description, OptionalFilePath imageFile)
	{
		ExerciseInfoId = exerciseInfoId;
		Index = index;
		Description = description;
		ImageFile = imageFile;
	}
}
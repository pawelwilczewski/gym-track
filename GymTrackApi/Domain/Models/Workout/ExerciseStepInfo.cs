using Domain.Common;

namespace Domain.Models.Workout;

public class ExerciseStepInfo
{
	public Id<ExerciseInfo> ExerciseInfoId { get; private set; }
	public int Index { get; private set; }

	public ExerciseInfo ExerciseInfo { get; private set; } = default!;

	public Description Description { get; private set; }
	public Option<FilePath> ImageFile { get; set; }

	private ExerciseStepInfo() { }

	public ExerciseStepInfo(Id<ExerciseInfo> exerciseInfoId, int index, Description description, Option<FilePath> imageFile)
	{
		ExerciseInfoId = exerciseInfoId;
		Index = index;
		Description = description;
		ImageFile = imageFile;
	}
}
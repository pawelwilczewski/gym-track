using Domain.Common;

namespace Domain.Models.Workout;

public class ExerciseInfo
{
	public Id<ExerciseInfo> Id { get; private set; } = Id<ExerciseInfo>.New();

	public Name Name { get; private set; }

	public FilePath ThumbnailImage { get; private set; }
	public Description Description { get; private set; }

	public ExerciseMetricType AllowedMetricTypes { get; private set; }

	public virtual List<ExerciseStepInfo> Steps { get; private set; } = [];
	public virtual List<Exercise> Exercises { get; private set; } = [];
	public virtual List<UserExerciseInfo> UserExerciseInfos { get; private set; } = [];

	private ExerciseInfo() { }

	public ExerciseInfo(Name name, FilePath thumbnailImage, Description description, ExerciseMetricType allowedMetricTypes)
	{
		Name = name;
		ThumbnailImage = thumbnailImage;
		Description = description;
		AllowedMetricTypes = allowedMetricTypes;
	}
}

[Flags]
public enum ExerciseMetricType
{
	Weight = 1 << 0,
	Duration = 1 << 1,
	Distance = 1 << 2
}
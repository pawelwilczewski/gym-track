using Domain.Common;

namespace Domain.Models.Workout;

public class ExerciseInfo
{
	public Id<ExerciseInfo> Id { get; set; } = Id<ExerciseInfo>.New();

	public Name Name { get; }

	public required FilePath ThumbnailImage { get; set; }
	public required string Description { get; set; }

	public required ExerciseMetricType AllowedMetricTypes { get; set; }

	public virtual List<ExerciseStepInfo> Steps { get; set; } = [];
	public virtual List<Exercise> Exercises { get; set; } = [];
	public virtual List<UserExerciseInfo> UserExerciseInfos { get; set; } = [];
}

[Flags]
public enum ExerciseMetricType
{
	Weight = 1 << 0,
	Duration = 1 << 1,
	Distance = 1 << 2
}
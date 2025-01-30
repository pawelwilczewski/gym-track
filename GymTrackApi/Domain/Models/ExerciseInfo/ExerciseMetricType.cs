namespace Domain.Models.ExerciseInfo;

[Flags]
public enum ExerciseMetricType
{
	None = 0,
	Weight = 1 << 0,
	Duration = 1 << 1,
	Distance = 1 << 2,
	All = Weight | Duration | Distance
}
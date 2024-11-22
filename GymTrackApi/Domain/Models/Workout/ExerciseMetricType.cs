namespace Domain.Models.Workout;

[Flags]
public enum ExerciseMetricType
{
	Weight = 1 << 0,
	Duration = 1 << 1,
	Distance = 1 << 2,
	All = Weight | Duration | Distance
}
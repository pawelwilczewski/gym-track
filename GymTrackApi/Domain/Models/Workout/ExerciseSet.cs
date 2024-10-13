namespace Domain.Models.Workout;

public class ExerciseSet
{
	public Id<Workout> WorkoutId { get; set; }
	public int ExerciseIndex { get; set; }
	public int SetIndex { get; set; }

	public virtual required Exercise Exercise { get; set; }

	public required ExerciseMetric Metric { get; set; }
	public int Reps { get; set; }
}
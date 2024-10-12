namespace Domain.Models.Workout;

public class Exercise
{
	public Id<Workout> WorkoutId { get; set; }
	public int Index { get; set; }

	public virtual Workout? Workout { get; set; }

	public Id<ExerciseInfo> ExerciseInfoId { get; set; }
	public virtual ExerciseInfo? ExerciseInfo { get; set; }

	public virtual List<ExerciseSet>? Sets { get; set; }
}
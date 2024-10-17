namespace Domain.Models.Workout;

public class Exercise
{
	public Id<Workout> WorkoutId { get; private set; }
	public int Index { get; private set; }

	public virtual Workout Workout { get; private set; } = default!;

	public Id<ExerciseInfo> ExerciseInfoId { get; private set; }
	public virtual ExerciseInfo ExerciseInfo { get; private set; } = default!;

	public virtual List<ExerciseSet> Sets { get; private set; } = [];

	// ReSharper disable once UnusedMember.Local
	private Exercise() { }

	public Exercise(Id<Workout> workoutId, int index, Id<ExerciseInfo> exerciseInfoId)
	{
		WorkoutId = workoutId;
		Index = index;
		ExerciseInfoId = exerciseInfoId;
	}
}
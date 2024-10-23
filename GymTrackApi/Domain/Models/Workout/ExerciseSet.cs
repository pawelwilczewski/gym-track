using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Workout;

public class ExerciseSet
{
	public Id<Workout> WorkoutId { get; private set; }
	public int ExerciseIndex { get; private set; }
	public int SetIndex { get; private set; }

	public virtual Workout.Exercise Exercise { get; private set; }

	public ExerciseMetric Metric { get; private set; }

	[Range(1, int.MaxValue, ErrorMessage = "Reps count can not be negative.")]
	public int Reps { get; private set; }

	private ExerciseSet() { }

	public ExerciseSet(Workout.Exercise exercise, int setIndex, ExerciseMetric metric, int reps)
	{
		WorkoutId = exercise.WorkoutId;
		ExerciseIndex = exercise.Index;
		SetIndex = setIndex;
		Exercise = exercise;
		Metric = metric;
		Reps = reps;
	}
}
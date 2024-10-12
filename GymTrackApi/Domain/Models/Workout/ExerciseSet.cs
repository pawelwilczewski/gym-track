namespace Domain.Models.Workout;

public class ExerciseSet
{
	public Id<Workout> WorkoutId { get; set; }
	public int ExerciseIndex { get; set; }
	public int SetIndex { get; set; }

	public virtual Exercise? Exercise { get; set; }

	public ExerciseMetric Metric { get; set; }
	public int Reps { get; set; }
}

public abstract record class ExerciseMetric;

public sealed record class Weight(double Value, Weight.Unit Units) : ExerciseMetric
{
	public enum Unit
	{
		Kilogram,
		Pound
	}
}

public sealed record class Duration(TimeSpan Time) : ExerciseMetric;

public sealed record class Distance(double Value, Distance.Unit Units) : ExerciseMetric
{
	public enum Unit
	{
		Metre,
		Yard
	}
}
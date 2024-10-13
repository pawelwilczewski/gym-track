namespace Domain.Models.Workout;

public class ExerciseSet
{
	public Id<Workout> WorkoutId { get; set; }
	public int ExerciseIndex { get; set; }
	public int SetIndex { get; set; }

	public virtual Exercise? Exercise { get; set; }

	public required ExerciseMetric Metric { get; set; }
	public int Reps { get; set; }
}

public abstract record class ExerciseMetric
{
	public abstract string Discriminator { get; }
}

public sealed record class Weight(double Value, Weight.Unit Units) : ExerciseMetric
{
	public override string Discriminator => nameof(Weight);

	public enum Unit
	{
		Kilogram,
		Pound
	}
}

public sealed record class Duration(TimeSpan Time) : ExerciseMetric
{
	public override string Discriminator => nameof(Duration);
}

public sealed record class Distance(double Value, Distance.Unit Units) : ExerciseMetric
{
	public override string Discriminator => nameof(Distance);

	public enum Unit
	{
		Metre,
		Yard
	}
}
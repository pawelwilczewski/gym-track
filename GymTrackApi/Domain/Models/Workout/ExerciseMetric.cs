namespace Domain.Models.Workout;

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
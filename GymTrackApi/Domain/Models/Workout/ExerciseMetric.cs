using System.Text.Json.Serialization;

namespace Domain.Models.Workout;

[JsonDerivedType(typeof(Weight), nameof(Weight))]
[JsonDerivedType(typeof(Distance), nameof(Distance))]
[JsonDerivedType(typeof(Duration), nameof(Duration))]
public abstract record class ExerciseMetric
{
	[JsonIgnore]
	public abstract ExerciseMetricType Type { get; }
}

public sealed record class Weight(
	Amount Value,
	Weight.Unit Units) : ExerciseMetric
{
	[JsonIgnore]
	public override ExerciseMetricType Type => ExerciseMetricType.Weight;

	public enum Unit
	{
		Kilogram,
		Pound
	}
}

public sealed record class Duration(
	TimeSpan Time) : ExerciseMetric
{
	[JsonIgnore]
	public override ExerciseMetricType Type => ExerciseMetricType.Duration;
}

public sealed record class Distance(
	Amount Value,
	Distance.Unit Units) : ExerciseMetric
{
	[JsonIgnore]
	public override ExerciseMetricType Type => ExerciseMetricType.Distance;

	public enum Unit
	{
		Metre,
		Yard
	}
}
using System.Text.Json.Serialization;

namespace Domain.Models.ExerciseInfo;

[JsonDerivedType(typeof(Weight), (int)ExerciseMetricType.Weight)]
[JsonDerivedType(typeof(Duration), (int)ExerciseMetricType.Duration)]
[JsonDerivedType(typeof(Distance), (int)ExerciseMetricType.Distance)]
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
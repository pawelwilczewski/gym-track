using System.Text.Json.Serialization;
using Domain.Common.ValueObjects;

namespace Domain.Models.ExerciseInfo;

[JsonDerivedType(typeof(Weight), (int)ExerciseMetricType.Weight)]
[JsonDerivedType(typeof(Duration), (int)ExerciseMetricType.Duration)]
[JsonDerivedType(typeof(Distance), (int)ExerciseMetricType.Distance)]
public abstract record class ExerciseMetric
{
	[JsonIgnore]
	public abstract SingleExerciseMetricType Type { get; }
}

public sealed record class Weight(
	WeightValue Value,
	Weight.Unit Units) : ExerciseMetric
{
	[JsonIgnore]
	public override SingleExerciseMetricType Type => SingleExerciseMetricType.From(ExerciseMetricType.Weight);

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
	public override SingleExerciseMetricType Type => SingleExerciseMetricType.From(ExerciseMetricType.Duration);
}

public sealed record class Distance(
	WeightValue Value,
	Distance.Unit Units) : ExerciseMetric
{
	[JsonIgnore]
	public override SingleExerciseMetricType Type => SingleExerciseMetricType.From(ExerciseMetricType.Distance);

	public enum Unit
	{
		Metre,
		Yard
	}
}
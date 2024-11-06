using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain.Models.Workout;

[DataContract]
[JsonDerivedType(typeof(Weight), nameof(Weight))]
[JsonDerivedType(typeof(Distance), nameof(Distance))]
[JsonDerivedType(typeof(Duration), nameof(Duration))]
public abstract record class ExerciseMetric
{
	public abstract ExerciseMetricType Type { get; }
}

[DataContract]
public sealed record class Weight(
	[property: DataMember] Amount Value,
	[property: DataMember] Weight.Unit Units) : ExerciseMetric
{
	public override ExerciseMetricType Type => ExerciseMetricType.Weight;

	public enum Unit
	{
		Kilogram,
		Pound
	}
}

[DataContract]
public sealed record class Duration(
	[property: DataMember] TimeSpan Time) : ExerciseMetric
{
	public override ExerciseMetricType Type => ExerciseMetricType.Duration;
}

[DataContract]
public sealed record class Distance(
	[property: DataMember] Amount Value,
	[property: DataMember] Distance.Unit Units) : ExerciseMetric
{
	public override ExerciseMetricType Type => ExerciseMetricType.Distance;

	public enum Unit
	{
		Metre,
		Yard
	}
}
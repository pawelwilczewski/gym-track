using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain.Models.Workout;

[DataContract]
[JsonDerivedType(typeof(Weight), (int)ExerciseMetricType.Weight)]
[JsonDerivedType(typeof(Distance), (int)ExerciseMetricType.Distance)]
[JsonDerivedType(typeof(Duration), (int)ExerciseMetricType.Duration)]
public abstract record class ExerciseMetric
{
	public abstract ExerciseMetricType Type { get; }
}

[DataContract]
[method: JsonConstructor]
public sealed record class Weight(
	[property: DataMember] double Value,
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
[method: JsonConstructor]
public sealed record class Duration(
	[property: DataMember] TimeSpan Time) : ExerciseMetric
{
	public override ExerciseMetricType Type => ExerciseMetricType.Duration;
}

[DataContract]
[method: JsonConstructor]
public sealed record class Distance(
	[property: DataMember] double Value,
	[property: DataMember] Distance.Unit Units) : ExerciseMetric
{
	public override ExerciseMetricType Type => ExerciseMetricType.Distance;

	public enum Unit
	{
		Metre,
		Yard
	}
}
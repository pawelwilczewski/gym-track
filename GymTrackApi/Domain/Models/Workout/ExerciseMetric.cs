using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Domain.Models.Workout;

[DataContract]
public abstract record class ExerciseMetric;

[DataContract, JsonDerivedType(typeof(ExerciseMetric), nameof(Weight))]
public sealed record class Weight(
	[property: DataMember] double Value,
	[property: DataMember] Weight.Unit Units) : ExerciseMetric
{
	public enum Unit
	{
		Kilogram,
		Pound
	}
}

[DataContract, JsonDerivedType(typeof(ExerciseMetric), nameof(Duration))]
public sealed record class Duration(
	[property: DataMember] TimeSpan Time) : ExerciseMetric;

[DataContract, JsonDerivedType(typeof(ExerciseMetric), nameof(Distance))]
public sealed record class Distance(
	[property: DataMember] double Value,
	[property: DataMember] Distance.Unit Units) : ExerciseMetric
{
	public enum Unit
	{
		Metre,
		Yard
	}
}
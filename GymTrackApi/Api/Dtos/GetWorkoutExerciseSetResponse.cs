using System.Runtime.Serialization;
using Domain.Models.Workout;

namespace Api.Dtos;

[DataContract]
public sealed record class GetWorkoutExerciseSetResponse(
	[property: DataMember] int Index,
	[property: DataMember] ExerciseMetric Metric,
	[property: DataMember] int Reps);
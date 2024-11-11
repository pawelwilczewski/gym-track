using System.Runtime.Serialization;
using Domain.Models.Workout;

namespace Api.Dtos;

[DataContract]
public sealed record class CreateWorkoutExerciseSetRequest(
	[property: DataMember] int Index,
	[property: DataMember] ExerciseMetric Metric,
	[property: DataMember] int Reps);
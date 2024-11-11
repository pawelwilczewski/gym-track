using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class GetWorkoutResponse(
	[property: DataMember] string Name,
	[property: DataMember] List<WorkoutExerciseKey> Exercises);
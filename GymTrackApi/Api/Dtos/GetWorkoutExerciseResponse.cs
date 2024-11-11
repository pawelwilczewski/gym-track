using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class GetWorkoutExerciseResponse(
	[property: DataMember] int Index,
	[property: DataMember] List<WorkoutExerciseSetKey> Sets);
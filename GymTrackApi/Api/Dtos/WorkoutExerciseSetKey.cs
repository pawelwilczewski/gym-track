using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class WorkoutExerciseSetKey(
	[property: DataMember] Guid WorkoutId,
	[property: DataMember] int ExerciseIndex,
	[property: DataMember] int Index);
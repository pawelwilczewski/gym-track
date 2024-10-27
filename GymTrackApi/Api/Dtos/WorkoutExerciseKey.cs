using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class WorkoutExerciseKey(
	[property: DataMember] Guid WorkoutId,
	[property: DataMember] int Index);
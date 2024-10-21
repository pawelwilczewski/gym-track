using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class ExerciseKey(
	[property: DataMember] Guid WorkoutId,
	[property: DataMember] int Index);
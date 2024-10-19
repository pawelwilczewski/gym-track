using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class GetWorkoutResponse(
	[property: DataMember] string Name,
	[property: DataMember] List<ExerciseKey> Exercises);

[DataContract]
internal sealed record class ExerciseKey(
	[property: DataMember] int Index);
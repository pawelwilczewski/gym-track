using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class CreateWorkoutExerciseRequest(
	[property: DataMember] int Index,
	[property: DataMember] Guid ExerciseInfoId);
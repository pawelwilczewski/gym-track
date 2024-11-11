using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class CreateWorkoutExerciseRequest(
	[property: DataMember] int Index,
	[property: DataMember] Guid ExerciseInfoId);
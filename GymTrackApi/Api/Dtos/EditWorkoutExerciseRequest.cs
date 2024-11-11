using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class EditWorkoutExerciseRequest(
	[property: DataMember] int Index);
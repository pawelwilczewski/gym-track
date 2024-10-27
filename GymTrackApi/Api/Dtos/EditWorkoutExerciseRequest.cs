using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class EditWorkoutExerciseRequest(
	[property: DataMember] int Index);
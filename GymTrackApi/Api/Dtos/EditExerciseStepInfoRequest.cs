using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class EditExerciseInfoStepRequest(
	[property: DataMember] string Description);
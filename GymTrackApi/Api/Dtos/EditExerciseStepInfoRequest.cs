using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class EditExerciseInfoStepRequest(
	[property: DataMember] string Description);
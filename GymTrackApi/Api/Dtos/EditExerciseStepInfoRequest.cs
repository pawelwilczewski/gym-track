using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class EditExerciseStepInfoRequest(
	[property: DataMember] string Description);
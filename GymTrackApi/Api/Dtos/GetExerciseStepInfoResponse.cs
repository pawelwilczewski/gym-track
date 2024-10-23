using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class GetExerciseInfoStepResponse(
	[property: DataMember] int Index,
	[property: DataMember] string Description,
	[property: DataMember] string? ImageUrl);
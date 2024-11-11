using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class GetExerciseInfoStepResponse(
	[property: DataMember] int Index,
	[property: DataMember] string Description,
	[property: DataMember] string? ImageUrl);
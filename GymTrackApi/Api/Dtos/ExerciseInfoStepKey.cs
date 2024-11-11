using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class ExerciseInfoStepKey(
	[property: DataMember] Guid ExerciseInfoId,
	[property: DataMember] int Index);
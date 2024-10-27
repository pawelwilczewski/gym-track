using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class ExerciseInfoStepKey(
	[property: DataMember] Guid ExerciseInfoId,
	[property: DataMember] int Index);
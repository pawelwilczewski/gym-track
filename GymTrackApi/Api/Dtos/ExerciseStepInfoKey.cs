using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class ExerciseStepInfoKey(
	[property: DataMember] Guid ExerciseInfoId,
	[property: DataMember] int Index);
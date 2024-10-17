using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class EditWorkoutRequest(
	[property: DataMember] string Name);
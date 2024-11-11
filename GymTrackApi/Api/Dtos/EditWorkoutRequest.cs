using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class EditWorkoutRequest(
	[property: DataMember] string Name);
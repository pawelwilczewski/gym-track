using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
public sealed record class CreateWorkoutRequest(
	[property: DataMember] string Name);
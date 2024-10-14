using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class CreateWorkoutRequest(
	[property: DataMember] string Name);
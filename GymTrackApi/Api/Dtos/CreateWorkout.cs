using System.Runtime.Serialization;

namespace Api.Dtos;

[DataContract]
internal sealed record class CreateWorkout(
	[property: DataMember] string Name);
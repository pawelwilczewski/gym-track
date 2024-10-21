using System.Runtime.Serialization;
using Domain.Models.Workout;

namespace Api.Dtos;

[DataContract]
internal sealed record class EditExerciseInfoRequest(
	[property: DataMember] string Name,
	[property: DataMember] string Description,
	[property: DataMember] ExerciseMetricType AllowedMetricTypes);
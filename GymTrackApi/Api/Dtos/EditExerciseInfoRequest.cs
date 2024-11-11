using System.Runtime.Serialization;
using Domain.Models.Workout;

namespace Api.Dtos;

[DataContract]
public sealed record class EditExerciseInfoRequest(
	[property: DataMember] string Name,
	[property: DataMember] string Description,
	[property: DataMember] ExerciseMetricType AllowedMetricTypes);
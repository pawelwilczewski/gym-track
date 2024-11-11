using System.Runtime.Serialization;
using Domain.Models.Workout;

namespace Api.Dtos;

[DataContract]
public sealed record class GetExerciseInfoResponse(
	[property: DataMember] string Name,
	[property: DataMember] string Description,
	[property: DataMember] ExerciseMetricType AllowedMetricTypes,
	[property: DataMember] string ThumbnailUrl,
	[property: DataMember] List<WorkoutExerciseKey> Exercises);
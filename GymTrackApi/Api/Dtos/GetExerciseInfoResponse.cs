using Domain.Models.Workout;

namespace Api.Dtos;

public sealed record class GetExerciseInfoResponse(
	string Name,
	string Description,
	ExerciseMetricType AllowedMetricTypes,
	string ThumbnailUrl,
	List<WorkoutExerciseKey> Exercises);
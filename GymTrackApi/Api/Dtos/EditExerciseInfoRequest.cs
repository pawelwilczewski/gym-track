using Domain.Models.Workout;

namespace Api.Dtos;

public sealed record class EditExerciseInfoRequest(
	string Name,
	string Description,
	ExerciseMetricType AllowedMetricTypes);
using Domain.Models.Workout;

namespace Api.Dtos;

public sealed record class GetWorkoutExerciseSetResponse(
	int Index,
	ExerciseMetric Metric,
	int Reps);
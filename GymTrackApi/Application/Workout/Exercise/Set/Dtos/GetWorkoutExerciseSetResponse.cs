using Domain.Models.ExerciseInfo;

namespace Application.Workout.Exercise.Set.Dtos;

public sealed record class GetWorkoutExerciseSetResponse(
	int Index,
	ExerciseMetric Metric,
	int Reps,
	int DisplayOrder);
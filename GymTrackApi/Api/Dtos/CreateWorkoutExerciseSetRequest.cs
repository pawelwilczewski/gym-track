using Domain.Models.Workout;

namespace Api.Dtos;

public sealed record class CreateWorkoutExerciseSetRequest(
	int Index,
	ExerciseMetric Metric,
	int Reps);
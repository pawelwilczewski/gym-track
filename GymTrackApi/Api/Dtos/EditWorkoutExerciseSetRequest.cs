using Domain.Models.Workout;

namespace Api.Dtos;

public sealed record class EditWorkoutExerciseSetRequest(
	ExerciseMetric Metric,
	int Reps);
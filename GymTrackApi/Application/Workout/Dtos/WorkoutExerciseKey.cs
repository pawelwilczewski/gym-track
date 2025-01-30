namespace Application.Workout.Dtos;

public sealed record class WorkoutExerciseKey(
	Guid WorkoutId,
	int ExerciseIndex);
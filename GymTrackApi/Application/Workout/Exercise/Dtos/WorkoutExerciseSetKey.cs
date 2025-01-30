namespace Application.Workout.Exercise.Dtos;

public sealed record class WorkoutExerciseSetKey(
	Guid WorkoutId,
	int ExerciseIndex,
	int SetIndex);
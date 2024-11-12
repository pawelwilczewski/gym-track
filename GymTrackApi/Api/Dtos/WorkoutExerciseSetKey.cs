namespace Api.Dtos;

public sealed record class WorkoutExerciseSetKey(
	Guid WorkoutId,
	int ExerciseIndex,
	int Index);
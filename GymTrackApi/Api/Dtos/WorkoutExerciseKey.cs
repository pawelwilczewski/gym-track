namespace Api.Dtos;

public sealed record class WorkoutExerciseKey(
	Guid WorkoutId,
	int Index);
namespace Api.Dtos;

public sealed record class GetWorkoutExerciseResponse(
	int Index,
	List<WorkoutExerciseSetKey> Sets);
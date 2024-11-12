namespace Api.Dtos;

public sealed record class GetWorkoutResponse(
	string Name,
	List<WorkoutExerciseKey> Exercises);
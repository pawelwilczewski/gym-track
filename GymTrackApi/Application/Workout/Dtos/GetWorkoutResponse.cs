namespace Application.Workout.Dtos;

public sealed record class GetWorkoutResponse(
	Guid Id,
	string Name,
	List<WorkoutExerciseKey> Exercises);
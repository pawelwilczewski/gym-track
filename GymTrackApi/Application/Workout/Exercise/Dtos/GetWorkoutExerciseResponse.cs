namespace Application.Workout.Exercise.Dtos;

public sealed record class GetWorkoutExerciseResponse(
	int Index,
	Guid ExerciseInfoId,
	int DisplayOrder,
	List<WorkoutExerciseSetKey> Sets);
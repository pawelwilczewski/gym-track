namespace Api.Dtos;

public sealed record class CreateWorkoutExerciseRequest(
	int Index,
	Guid ExerciseInfoId);
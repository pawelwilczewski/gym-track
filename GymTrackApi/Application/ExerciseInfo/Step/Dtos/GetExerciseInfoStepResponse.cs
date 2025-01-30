namespace Application.ExerciseInfo.Step.Dtos;

public sealed record class GetExerciseInfoStepResponse(
	int Index,
	string Description,
	string? ImageUrl,
	int DisplayOrder);
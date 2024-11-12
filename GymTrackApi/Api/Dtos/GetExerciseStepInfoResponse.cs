namespace Api.Dtos;

public sealed record class GetExerciseInfoStepResponse(
	int Index,
	string Description,
	string? ImageUrl);
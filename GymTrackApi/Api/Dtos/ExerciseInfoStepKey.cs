namespace Api.Dtos;

public sealed record class ExerciseInfoStepKey(
	Guid ExerciseInfoId,
	int Index);
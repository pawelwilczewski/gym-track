namespace Application.ExerciseInfo.Dtos;

public sealed record class ExerciseInfoStepKey(
	Guid ExerciseInfoId,
	int StepIndex);
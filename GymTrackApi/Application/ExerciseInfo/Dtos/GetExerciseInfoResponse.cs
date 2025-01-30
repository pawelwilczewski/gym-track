using Domain.Models.ExerciseInfo;

namespace Application.ExerciseInfo.Dtos;

public sealed record class GetExerciseInfoResponse(
	Guid Id,
	string Name,
	string Description,
	ExerciseMetricType AllowedMetricTypes,
	string? ThumbnailUrl,
	List<ExerciseInfoStepKey> Steps);
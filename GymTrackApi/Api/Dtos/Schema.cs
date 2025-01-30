using Domain.Models.ExerciseInfo;

namespace Api.Dtos;

public sealed record class CreateWorkoutExerciseRequest(
	Guid ExerciseInfoId);

public sealed record class CreateWorkoutExerciseSetRequest(
	ExerciseMetric Metric,
	int Reps);

public sealed record class CreateWorkoutRequest(
	string Name);

public sealed record class UpdateWorkoutExerciseSetRequest(
	ExerciseMetric Metric,
	int Reps);

public sealed record class UpdateDisplayOrderRequest(
	int DisplayOrder);

public sealed record class UpdateWorkoutRequest(
	string Name);

public sealed record class GetAntiforgeryTokenResponse(
	string Token);

public sealed record class CreateTrackedWorkoutRequest(
	Guid WorkoutId,
	DateTime PerformedAt,
	TimeSpan Duration);

public sealed record class UpdateTrackedWorkoutRequest(
	DateTime PerformedAt,
	TimeSpan Duration);
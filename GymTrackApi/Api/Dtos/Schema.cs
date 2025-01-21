using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;

namespace Api.Dtos;

public sealed record class CreateWorkoutExerciseRequest(
	Guid ExerciseInfoId);

public sealed record class CreateWorkoutExerciseSetRequest(
	ExerciseMetric Metric,
	int Reps);

public sealed record class CreateWorkoutRequest(
	string Name);

public sealed record class EditWorkoutExerciseSetRequest(
	ExerciseMetric Metric,
	int Reps);

public sealed record class EditDisplayOrderRequest(
	int DisplayOrder);

public sealed record class EditWorkoutRequest(
	string Name);

public sealed record class ExerciseInfoStepKey(
	Guid ExerciseInfoId,
	int StepIndex);

public sealed record class GetExerciseInfoResponse(
	Guid Id,
	string Name,
	string Description,
	ExerciseMetricType AllowedMetricTypes,
	string? ThumbnailUrl,
	List<ExerciseInfoStepKey> Steps);

public sealed record class GetExerciseInfoStepResponse(
	int Index,
	string Description,
	string? ImageUrl,
	int DisplayOrder);

public sealed record class GetWorkoutExerciseResponse(
	int Index,
	Guid ExerciseInfoId,
	int DisplayOrder,
	List<WorkoutExerciseSetKey> Sets);

public sealed record class GetWorkoutExerciseSetResponse(
	int Index,
	ExerciseMetric Metric,
	int Reps,
	int DisplayOrder);

public sealed record class GetWorkoutResponse(
	Guid Id,
	string Name,
	List<WorkoutExerciseKey> Exercises);

public sealed record class WorkoutExerciseKey(
	Guid WorkoutId,
	int ExerciseIndex);

public sealed record class WorkoutExerciseSetKey(
	Guid WorkoutId,
	int ExerciseIndex,
	int SetIndex);
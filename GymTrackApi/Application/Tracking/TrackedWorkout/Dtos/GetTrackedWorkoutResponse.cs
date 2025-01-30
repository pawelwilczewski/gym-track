namespace Application.Tracking.TrackedWorkout.Dtos;

public sealed record class GetTrackedWorkoutResponse(
	Guid Id,
	Guid WorkoutId,
	DateTime PerformedAt,
	TimeSpan Duration);
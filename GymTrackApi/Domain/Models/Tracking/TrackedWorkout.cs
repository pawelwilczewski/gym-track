using Domain.Models.Identity;

namespace Domain.Models.Tracking;

public class TrackedWorkout
{
	public Id<TrackedWorkout> Id { get; } = Id<TrackedWorkout>.New();

	public Id<Workout.Workout> WorkoutId { get; }
	public virtual Workout.Workout Workout { get; private set; } = default!;

	public Guid UserId { get; }
	public virtual User User { get; private set; } = default!;

	public DateTime PerformedAt { get; set; }
	public TimeSpan Duration { get; set; }

	public TrackedWorkout() { }

	public TrackedWorkout(Id<Workout.Workout> workoutId, Guid userId, DateTime performedAt, TimeSpan duration)
	{
		WorkoutId = workoutId;
		UserId = userId;
		PerformedAt = performedAt;
		Duration = duration;
	}
}
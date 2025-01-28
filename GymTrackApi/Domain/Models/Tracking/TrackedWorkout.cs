using Domain.Common.Exceptions;
using Domain.Common.Ownership;

namespace Domain.Models.Tracking;

public class TrackedWorkout : IOwned
{
	public Id<TrackedWorkout> Id { get; } = Id<TrackedWorkout>.New();

	public Id<Workout.Workout> WorkoutId { get; }
	public virtual Workout.Workout Workout { get; private set; } = default!;

	public DateTime PerformedAt { get; set; }
	public TimeSpan Duration { get; set; }

	// ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
	public Guid? OwnerId { get; private set; }
	public Owner Owner => OwnerId;

	private TrackedWorkout() { }

	public TrackedWorkout(Id<Workout.Workout> workoutId, DateTime performedAt, TimeSpan duration, Guid userId)
	{
		WorkoutId = workoutId;
		PerformedAt = performedAt;
		Duration = duration;
		OwnerId = userId;
	}

	public void Update(DateTime performedAt, TimeSpan duration, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		PerformedAt = performedAt;
		Duration = duration;
	}
}
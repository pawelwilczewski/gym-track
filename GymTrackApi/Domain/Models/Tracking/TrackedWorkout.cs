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

	public Owner Owner => ownerId;

	// ReSharper disable once FieldCanBeMadeReadOnly.Local
	private Guid? ownerId;

	private TrackedWorkout() { }

	public TrackedWorkout(Id<Workout.Workout> workoutId, DateTime performedAt, TimeSpan duration, Guid userId)
	{
		WorkoutId = workoutId;
		PerformedAt = performedAt;
		Duration = duration;
		ownerId = userId;
	}

	public void Update(DateTime performedAt, TimeSpan duration, Guid userId)
	{
		if (!this.CanBeModifiedBy(userId)) throw new PermissionError();

		PerformedAt = performedAt;
		Duration = duration;
	}
}
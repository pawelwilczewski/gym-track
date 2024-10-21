using Domain.Models.Identity;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

public class UserWorkout : IUserOwned
{
	public Guid UserId { get; private set; }
	public Id<Workout> WorkoutId { get; private set; }

	public virtual User User { get; private set; } = default!;
	public virtual Workout Workout { get; private set; } = default!;

	public UserWorkout() { }

	public UserWorkout(Guid userId, Id<Workout> workoutId)
	{
		UserId = userId;
		WorkoutId = workoutId;
	}
}
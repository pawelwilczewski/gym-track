using Domain.Models.Identity;

namespace Domain.Models.Workout;

public class UserWorkout
{
	public Guid UserId { get; set; }
	public Id<Workout> WorkoutId { get; set; }

	public virtual required User User { get; set; }
	public virtual required Workout Workout { get; set; }
}
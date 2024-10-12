using Domain.Models.Identity;

namespace Domain.Models.Workout;

public class UserWorkout
{
	public Guid UserId { get; set; }
	public Id<Workout> WorkoutId { get; set; }

	public virtual User? User { get; set; }
	public virtual Workout? Workout { get; set; }
}
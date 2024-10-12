using Domain.Models.Identity;

namespace Domain.Models.Workout;

public class UserWorkout
{
	public Guid UserId { get; set; }
	public Id<Workout> WorkoutId { get; set; }

	public virtual AppUser? User { get; set; }
	public virtual Workout? Workout { get; set; }
}
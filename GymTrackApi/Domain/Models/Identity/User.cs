using Domain.Models.Workout;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Identity;

public class User : IdentityUser<Guid>
{
	public virtual List<UserWorkout> Workouts { get; set; } = [];
}
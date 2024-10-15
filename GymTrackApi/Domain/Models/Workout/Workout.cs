using System.Security.Claims;
using Domain.Common;

namespace Domain.Models.Workout;

public class Workout
{
	public Id<Workout> Id { get; set; } = Id<Workout>.New();

	public string Name { get; private set; }

	public virtual List<UserWorkout> UserWorkouts { get; set; } = [];
	public virtual List<Exercise> Exercises { get; set; } = [];

	private Workout(string name) => Name = name;

	public static Workout CreateDefault(string name) => new(name);

	public static Workout CreateForUser(string name, ClaimsPrincipal principal)
	{
		var workout = new Workout(name);
		var userWorkout = new UserWorkout(principal.GetUserId(), workout.Id);
		workout.UserWorkouts.Add(userWorkout);

		return workout;
	}
}
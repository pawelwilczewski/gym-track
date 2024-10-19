using System.Security.Claims;
using Domain.Common;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

public class Workout
{
	public Id<Workout> Id { get; private set; } = Id<Workout>.New();

	public Name Name { get; private set; }

	public virtual List<UserWorkout> Users { get; private set; } = [];
	public virtual List<Exercise> Exercises { get; private set; } = [];

	private Workout(Name name) => Name = name;

	public static Workout CreateForEveryone(Name name) => new(name);

	public static Workout CreateForUser(Name name, ClaimsPrincipal user)
	{
		var workout = new Workout(name);
		var userWorkout = new UserWorkout(user.GetUserId(), workout.Id);
		workout.Users.Add(userWorkout);

		return workout;
	}
}
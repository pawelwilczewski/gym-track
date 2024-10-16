using System.Security.Claims;
using Domain.Common;
using Domain.Models.Identity;

namespace Domain.Models.Workout;

public class Workout
{
	public Id<Workout> Id { get; set; } = Id<Workout>.New();

	public Name Name { get; private set; }

	public virtual List<UserWorkout> UserWorkouts { get; set; } = [];
	public virtual List<Exercise> Exercises { get; set; } = [];

	private Workout(Name name) => Name = name;

	public CanDeleteResult CanDelete(ClaimsPrincipal user)
	{
		switch (UserWorkouts)
		{
			// in case there are no user workouts associated,
			// this is a template workout - can be only deleted by admins
			case []:
			{
				if (!user.IsInRole(Role.ADMINISTRATOR)) return CanDeleteResult.Unauthorized;

				break;
			}
			case [var userWorkout]:
			{
				if (userWorkout.UserId != user.GetUserId()) return CanDeleteResult.NotFound;

				break;
			}
			case [..]: return CanDeleteResult.CantDeleteShared;
		}

		return CanDeleteResult.Yes;
	}

	public static Workout CreateDefault(Name name) => new(name);

	public static Workout CreateForUser(Name name, ClaimsPrincipal user)
	{
		var workout = new Workout(name);
		var userWorkout = new UserWorkout(user.GetUserId(), workout.Id);
		workout.UserWorkouts.Add(userWorkout);

		return workout;
	}

	public enum CanDeleteResult
	{
		Yes,
		Unauthorized,
		NotFound,
		CantDeleteShared
	}
}
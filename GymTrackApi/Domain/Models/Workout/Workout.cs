using System.Security.Claims;
using Domain.Common;
using Domain.Models.Identity;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.Workout;

public class Workout
{
	public Id<Workout> Id { get; private set; } = Id<Workout>.New();

	public Name Name { get; private set; }

	public virtual List<UserWorkout> UserWorkouts { get; private set; } = [];
	public virtual List<Exercise> Exercises { get; private set; } = [];

	private Workout(Name name) => Name = name;

	public static Workout CreateForEveryone(Name name) => new(name);

	public static Workout CreateForUser(Name name, ClaimsPrincipal user)
	{
		var workout = new Workout(name);
		var userWorkout = new UserWorkout(user.GetUserId(), workout.Id);
		workout.UserWorkouts.Add(userWorkout);

		return workout;
	}

	public CanModifyResult CanDeleteOrModify(ClaimsPrincipal user)
	{
		switch (UserWorkouts)
		{
			// in case there are no user workouts associated,
			// this is a template workout - can be only deleted by admins
			case []:
			{
				if (!user.IsInRole(Role.ADMINISTRATOR)) return CanModifyResult.Unauthorized;

				break;
			}
			case [var userWorkout]:
			{
				if (userWorkout.UserId != user.GetUserId()) return CanModifyResult.NotFound;

				break;
			}
			case [..]: return CanModifyResult.ProhibitShared;
		}

		return CanModifyResult.Yes;
	}

	public enum CanModifyResult
	{
		Yes,
		Unauthorized,
		NotFound,
		ProhibitShared
	}
}
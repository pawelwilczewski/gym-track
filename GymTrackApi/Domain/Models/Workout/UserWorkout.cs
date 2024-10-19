using System.Security.Claims;
using Domain.Common;
using Domain.Models.Common;
using Domain.Models.Identity;

namespace Domain.Models.Workout;

public class UserWorkout
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

public static class UserWorkoutExtensions
{
	public static CanModifyResult CanModifyOrDeleteWorkout(
		this ClaimsPrincipal user,
		IReadOnlyList<UserWorkout> workoutUsers)
	{
		switch (workoutUsers)
		{
			// in case there are no user workouts associated,
			// this is a template workout - can be only deleted by admins
			case []:
			{
				if (!user.IsInRole(Role.ADMINISTRATOR)) return new CanModifyResult.Unauthorized();

				break;
			}
			case [var userWorkout]:
			{
				if (userWorkout.UserId != user.GetUserId()) return new CanModifyResult.NotFound();

				break;
			}
			case [..]: return new CanModifyResult.ProhibitShared();
		}

		return new CanModifyResult.Ok();
	}

	public static bool CanAccessWorkout(
		this ClaimsPrincipal user,
		IReadOnlyList<UserWorkout> workoutUsers)
	{
		var userId = user.GetUserId();
		return workoutUsers.Count == 0
			|| workoutUsers.FirstOrDefault(userWorkout => userWorkout.UserId == userId) is not null;
	}
}
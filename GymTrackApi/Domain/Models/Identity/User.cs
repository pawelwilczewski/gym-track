using System.Security.Claims;
using Domain.Common;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Identity;

public class User : IdentityUser<Guid>
{
	public virtual List<UserWorkout> Workouts { get; set; } = [];
	public virtual List<UserExerciseInfo> ExerciseInfos { get; set; } = [];
}

public interface IUserOwned
{
	Guid UserId { get; }
}

public static class UserOwnedExtensions
{
	public static CanModifyResult CanModifyOrDelete(
		this ClaimsPrincipal user,
		IReadOnlyList<IUserOwned> workoutUsers)
	{
		switch (workoutUsers)
		{
			// in case there are no user owned items,
			// this is a template item - can be only deleted by admins
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

	public static bool CanAccess(
		this ClaimsPrincipal user,
		IReadOnlyList<IUserOwned> workoutUsers)
	{
		var userId = user.GetUserId();
		return workoutUsers.Count == 0
			|| workoutUsers.FirstOrDefault(userWorkout => userWorkout.UserId == userId) is not null;
	}
}
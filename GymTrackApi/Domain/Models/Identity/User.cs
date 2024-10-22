using System.Diagnostics.CodeAnalysis;
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
	public static bool CanModifyOrDelete(
		this ClaimsPrincipal user,
		IReadOnlyList<IUserOwned> users,
		[NotNullWhen(false)] out CantModifyReason? reason)
	{
		switch (users)
		{
			// in case there are no user owned items,
			// this is a template item - can be only deleted by admins
			case []:
			{
				if (!user.IsInRole(Role.ADMINISTRATOR))
				{
					reason = new CantModifyReason.Unauthorized();
					return false;
				}

				break;
			}
			case [var userWorkout]:
			{
				if (userWorkout.UserId != user.GetUserId())
				{
					reason = new CantModifyReason.NotFound();
					return false;
				}

				break;
			}
			case [..]:
			{
				reason = new CantModifyReason.ProhibitShared();
				return false;
			}
		}

		reason = null;
		return true;
	}

	public static bool CanAccess(
		this ClaimsPrincipal user,
		IReadOnlyList<IUserOwned> users)
	{
		var userId = user.GetUserId();
		return users.Count == 0
			|| users.FirstOrDefault(userWorkout => userWorkout.UserId == userId) is not null;
	}
}
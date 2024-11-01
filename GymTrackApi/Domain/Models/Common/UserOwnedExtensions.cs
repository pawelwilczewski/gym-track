using System.Security.Claims;
using Domain.Common;
using Domain.Models.Identity;

namespace Domain.Models.Common;

public static class UserOwnedExtensions
{
	public static bool CanModifyOrDelete(
		this ClaimsPrincipal user,
		IReadOnlyList<IUserOwned> users)
	{
		switch (users)
		{
			// in case there are no user owned items,
			// this is a template item - can be only deleted by admins
			case []:
			{
				if (!user.IsInRole(Role.ADMINISTRATOR)) return false;

				break;
			}
			case [var userWorkout]:
			{
				if (userWorkout.UserId != user.GetUserId()) return false;

				break;
			}
			case [..]:
			{
				return false;
			}
		}

		return true;
	}

	public static bool CanAccess(
		this ClaimsPrincipal user,
		IReadOnlyList<IUserOwned> users)
	{
		if (users.Count <= 0) return true;

		var userId = user.GetUserId();
		return users.FirstOrDefault(userWorkout => userWorkout.UserId == userId) is not null;
	}
}
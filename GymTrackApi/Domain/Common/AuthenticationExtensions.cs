using System.Security.Claims;
using Domain.Models.User;

namespace Domain.Common;

public static class AuthenticationExtensions
{
	public static UserId GetUserId(this ClaimsPrincipal principal)
	{
		var claim = principal.FindFirst(ClaimTypes.NameIdentifier)!;
		return UserId.From(new Guid(claim.Value));
	}
}
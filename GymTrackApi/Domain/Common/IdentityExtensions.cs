using System.Security.Claims;

namespace Domain.Common;

public static class IdentityExtensions
{
	public static Guid GetUserId(this ClaimsPrincipal principal)
	{
		var claim = principal.FindFirst(ClaimTypes.NameIdentifier)!;
		return new Guid(claim.Value);
	}
}
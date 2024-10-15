using Domain.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

internal static class Policy
{
	public const string REQUIRE_ADMINISTRATOR_ROLE = "RequireAdministratorRole";

	public static void AddPolicies(this AuthorizationBuilder builder)
	{
		builder.AddPolicy(REQUIRE_ADMINISTRATOR_ROLE,
			policy => policy.RequireRole(Role.ADMINISTRATOR));
	}
}
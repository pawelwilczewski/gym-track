using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Api.Authorization;

public static class Roles
{
	public const string ADMINISTRATOR = "Administrator";

	public static async Task AddRoles(this IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

		if (!await roleManager.RoleExistsAsync(ADMINISTRATOR))
		{
			await roleManager.CreateAsync(new Role(ADMINISTRATOR));
		}
	}
}
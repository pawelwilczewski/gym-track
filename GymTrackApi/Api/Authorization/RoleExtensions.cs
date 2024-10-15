using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Api.Authorization;

public static class RoleExtensions
{
	public static async Task AddRoles(this IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

		if (!await roleManager.RoleExistsAsync(Role.ADMINISTRATOR))
		{
			await roleManager.CreateAsync(new Role(Role.ADMINISTRATOR));
		}
	}
}
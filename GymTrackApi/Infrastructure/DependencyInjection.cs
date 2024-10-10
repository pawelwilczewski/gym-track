using Application.Persistence;
using Domain.Models.User;
using Infrastructure.Persistence;
using Infrastructure.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
	{
		services
			.AddDbContext<AppDbContext>()
			.AddIdentityCore<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddEntityFrameworkStores<AppDbContext>()
			.AddUserManager<UserManager>()
			.AddUserStore<UserStore<AppUser, Role, AppDbContext, Guid>>()
			.AddRoles<Role>()
			.AddRoleStore<RoleStore<Role, AppDbContext, Guid>>();

		services.AddScoped<IDataContext, DataContext>();

		return services;
	}
}
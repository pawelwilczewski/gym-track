using Application.Persistence;
using Domain.Models.User;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddDbContext<AppDbContext>(options => options
				.UseNpgsql(configuration.GetConnectionString("AppDb"))
				.EnableSensitiveDataLogging())
			.AddScoped<IDataContext, DataContext>();

		services
			.AddIdentityCore<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddRoles<Role>()
			.AddEntityFrameworkStores<AppDbContext>();

		return services;
	}
}
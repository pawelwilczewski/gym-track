using Application.Persistence;
using Domain.Models.Identity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("AppDb");

		services
			.AddDbContext<IDataContext, AppDbContext>(options =>
			{
				options
					.UseNpgsql(connectionString);

				if (bool.TryParse(configuration["EnableSensitiveDataLogging"], out var enable) && enable)
				{
					options.EnableSensitiveDataLogging();
				}
			});

		services
			.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddRoles<Role>()
			.AddEntityFrameworkStores<AppDbContext>();

		return services;
	}
}
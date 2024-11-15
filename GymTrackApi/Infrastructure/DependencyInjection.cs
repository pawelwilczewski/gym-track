using Application.Persistence;
using Domain.Models.Identity;
using Infrastructure.Email;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		var dbSection = configuration.GetRequiredSection("Database");
		var connectionString = dbSection["ConnectionString"];

		services
			.AddDbContext<IDataContext, AppDbContext>(options =>
			{
				options
					.UseNpgsql(connectionString);

				if (bool.TryParse(dbSection["EnableSensitiveDataLogging"], out var enable) && enable)
				{
					options.EnableSensitiveDataLogging();
				}
			});

		services
			.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddRoles<Role>()
			.AddEntityFrameworkStores<AppDbContext>();

		services.AddSingleton<IEmailSender<User>, SendGridIdentityEmailSender>();

		return services;
	}
}
using Application.Auth.Abstractions;
using Application.Email;
using Application.Persistence;
using Infrastructure.Authentication;
using Infrastructure.Email;
using Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddMediatR(config =>
			config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

		services
			.AddDbContext<AppDbContext>(options =>
			{
				// TODO Pawel: look into IOptions - is it applicable to simplify this parsing?
				var dbSection = configuration.GetRequiredSection("Database");

				options
					.UseNpgsql(dbSection["ConnectionString"]);

				if (bool.TryParse(dbSection["EnableSensitiveDataLogging"], out var enable) && enable)
				{
					options.EnableSensitiveDataLogging();
				}
			})
			.AddScoped<IUsersDataContext, UsersDataContext>()
			.AddScoped<IUserDataContextFactory, UserDataContextFactory>();

		services.AddSingleton<IPasswordHasher, PasswordHasher>();
		services.AddSingleton<IPasswordVerifier, PasswordVerifier>();

		services.AddSingleton<IEmailSender, SendGridEmailSender>();
		services.AddSingleton<IUserEmailSender, UserEmailSender>();

		services.AddSingleton<ITokenProvider, TokenProvider>();

		services.AddMassTransit(configurator =>
		{
			configurator.UsingInMemory((context, busConfigurator) =>
				busConfigurator.ConfigureEndpoints(context));

			configurator.AddEntityFrameworkOutbox<AppDbContext>(efConfigurator =>
			{
				efConfigurator.QueryDelay = TimeSpan.FromSeconds(10);
				efConfigurator.UsePostgres();
				efConfigurator.UseBusOutbox();
			});
		});

		return services;
	}
}
using Application.Auth.Abstractions;
using Application.Email;
using Application.Persistence;
using Infrastructure.Authentication;
using Infrastructure.Email;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using Infrastructure.Settings;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddMediatR(config =>
			config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

		services.AddScoped<PublishDomainEventsInterceptor>();

		services
			.AddDbContext<AppDbContext>((sp, options) =>
			{
				var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;

				options.UseNpgsql(settings.ConnectionString);

				if (settings.EnableSensitiveDataLogging)
				{
					options.EnableSensitiveDataLogging();
				}

				options.AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>());
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
			{
				busConfigurator.ConfigureEndpoints(context);
				busConfigurator.UseRawJsonDeserializer();
				busConfigurator.UseRawJsonSerializer();
				busConfigurator.UseMessageRetry(retryConfigure =>
					retryConfigure.Interval(4, TimeSpan.FromSeconds(1)));
			});

			configurator.AddEntityFrameworkOutbox<AppDbContext>(efConfigurator =>
			{
				efConfigurator.QueryDelay = TimeSpan.FromSeconds(10);
				efConfigurator.UsePostgres();
				efConfigurator.UseBusOutbox();
			});

			configurator.AddConsumer<DomainEventMessageConsumer>();
		});

		services.Configure<EmailConfirmationSettings>(configuration.GetSection("EmailConfirmation"));
		services.Configure<FrontendSettings>(configuration.GetSection("Frontend"));

		services.AddSingleton<IEmailConfirmationCodeGenerator, EmailConfirmationCodeGenerator>();

		services.Configure<SendGridSettings>(configuration.GetSection("SendGrid"));
		services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
		services.Configure<DatabaseSettings>(configuration.GetSection("Database"));

		return services;
	}

	public static async Task ConfigureAppInfrastructure(this IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();

		await DbInitialization.InitializeDb(
				scope.ServiceProvider.GetRequiredService<AppDbContext>(),
				serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>())
			.ConfigureAwait(false);
	}
}
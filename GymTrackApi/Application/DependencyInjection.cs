using Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddMediatR(config =>
			config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

		services.Configure<VersionSettings>(configuration.GetSection("Version"));
		services.Configure<OpenApiSettings>(configuration.GetSection("OpenApi"));
		services.Configure<EmailConfirmationSettings>(configuration.GetSection("EmailConfirmation"));
		services.Configure<FrontendSettings>(configuration.GetSection("Frontend"));
		services.Configure<EmailSenderSettings>(configuration.GetSection("EmailSender"));
		services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
		services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
		services.Configure<PasswordResetSettings>(configuration.GetSection("PasswordReset"));

		return services;
	}
}
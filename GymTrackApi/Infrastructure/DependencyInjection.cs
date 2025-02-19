using System.Text;
using Application.Auth.Abstractions;
using Application.Email;
using Application.Persistence;
using Application.Settings;
using Infrastructure.Authentication;
using Infrastructure.Email;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
	{
		// TODO Pawel: clean all of this up and order + split up accordingly

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

		services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();

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

		services.AddSingleton<IEmailConfirmationCodeGenerator, EmailConfirmationCodeGenerator>();
		services.AddSingleton<IPasswordResetCodeGenerator, PasswordResetCodeGenerator>();
		services.AddSingleton<IRefreshTokenProvider, RefreshTokenProvider>();

		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);
		services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
			.Configure<IOptions<JwtSettings>, IOptions<FrontendSettings>, IOptions<OpenApiSettings>>(
				(options, jwtSettings, frontendSettings, openApiSettings) =>
				{
					List<string> audiences = [frontendSettings.Value.BaseUrl];
					if (openApiSettings.Value.Enabled)
					{
						audiences.Add(openApiSettings.Value.Url);
					}

					options.TokenValidationParameters = new TokenValidationParameters
					{
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Key)),
						ValidIssuer = jwtSettings.Value.Issuer,
						ValidAudiences = audiences,
						ClockSkew = TimeSpan.Zero
					};
				});

		services.AddAuthorization();

		return services;
	}

	public static async Task ConfigureAppInfrastructure(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();

		await DbInitialization.InitializeDb(
				scope.ServiceProvider.GetRequiredService<AppDbContext>(),
				scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>())
			.ConfigureAwait(false);
	}
}
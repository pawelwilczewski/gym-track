using Api.Common;
using Api.Files;
using Api.Middleware;
using Api.Routes;
using Application;
using Application.Persistence;
using Application.Settings;
using Asp.Versioning;
using Infrastructure;
using Infrastructure.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var versionSettings = builder.Configuration.GetRequiredSection("Version");
var apiVersion = new ApiVersion(
	int.Parse(versionSettings[nameof(VersionSettings.Major)]!),
	int.Parse(versionSettings[nameof(VersionSettings.Minor)]!));
const string apiVersionGroupNameFormat = "'v'VVV";

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
		policy =>
		{
			policy.WithOrigins("https://localhost:7173", "https://localhost:7050")
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials()
				.WithExposedHeaders("Location");
		});
});

builder.Services
	.AddEndpointsApiExplorer();

builder.Services.AddAntiforgery(options =>
{
	options.FormFieldName = "__RequestVerificationToken";
	options.HeaderName = "X-CSRF-TOKEN";
});

if (bool.TryParse(builder.Configuration.GetRequiredSection("OpenApi")[nameof(OpenApiSettings.Enabled)],
		out var openApiEnabled)
	&& openApiEnabled)
{
	builder.Services.AddOpenApi(apiVersion.ToString(apiVersionGroupNameFormat));
}

builder.Services
	.AddApplicationDependencies(builder.Configuration)
	.AddInfrastructureDependencies();

builder.Services.AddApiVersioning(options =>
	{
		options.DefaultApiVersion = apiVersion;
		options.ApiVersionReader = new UrlSegmentApiVersionReader();
	})
	.AddApiExplorer(options =>
	{
		options.GroupNameFormat = apiVersionGroupNameFormat;
		options.SubstituteApiVersionInUrl = true;
	});

builder.Services.AddSingleton<IFileStoragePathProvider, WebRootFileStoragePathProvider>();

builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.Configure());

var app = builder.Build();

app.UseCors();

await app.ConfigureAppInfrastructure().ConfigureAwait(false);

var openApiSettings = app.Services.GetRequiredService<IOptions<OpenApiSettings>>();
if (openApiSettings.Value.Enabled)
{
	app.MapOpenApi();
	app.MapScalarApiReference(options =>
	{
		options.Title = "Gym Track API";

		options.Authentication = new ScalarAuthenticationOptions
		{
			PreferredSecurityScheme = JwtBearerDefaults.AuthenticationScheme
		};
	});
}

if (app.Environment.IsProduction()) // TODO Pawel: IsProductionOrTest()? <- instead make this configurable in appsettings
{
	app.Strip404Body();
	app.Use404InsteadOf403();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAntiforgery();

app.AddPutFormSupport();

app.UseAuthentication();
app.UseAuthorization();

app.MapAllRoutes();

app.Run();

public partial class Program; // for functional tests
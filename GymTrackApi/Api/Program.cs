using System.Text;
using Api.Authorization;
using Api.Common;
using Api.Files;
using Api.Middleware;
using Api.Routes;
using Application;
using Application.Persistence;
using Asp.Versioning;
using Domain.Models.Identity;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var apiVersion = new ApiVersion(1);
const string apiVersionGroupNameFormat = "'v'V";

var builder = WebApplication.CreateBuilder(args);

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

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Gym Track API",
		Version = apiVersion.ToString(apiVersionGroupNameFormat)
	}));
}

builder.Services.AddAntiforgery(options =>
{
	options.FormFieldName = "__RequestVerificationToken";
	options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services
	.AddAuthentication(IdentityConstants.BearerScheme)
	.AddJwtBearer(options =>
	{
		var jwtSettings = builder.Configuration.GetSection("Jwt");
		var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true, // TODO Pawel: doesn't seem to validate audience correctly!
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtSettings["Issuer"],
			ValidAudience = jwtSettings["Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(key),
			ClockSkew = TimeSpan.Zero
		};
	});
builder.Services.AddAuthorizationBuilder()
	.AddPolicies();

builder.Services
	.AddApplicationDependencies(builder.Configuration)
	.AddInfrastructureDependencies(builder.Configuration);

builder.Services
	.AddIdentityApiEndpoints<User>()
	.AddDefaultTokenProviders();

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

await app.Services.InitializeDb(builder.Configuration).ConfigureAwait(false);
await app.Services.AddRoles().ConfigureAwait(false);

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

if (app.Environment.IsProduction()) // TODO Pawel: IsProductionOrTest()?
{
	app.Strip404Body();
	app.Use404InsteadOf403();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
app.UseAntiforgery();

app.AddPutFormSupport();

app.MapAllRoutes();

app.Run();

public partial class Program; // for functional tests
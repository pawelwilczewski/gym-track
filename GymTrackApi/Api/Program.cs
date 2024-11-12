using Api.Authorization;
using Api.Common;
using Api.Files;
using Api.Routes;
using Application.Persistence;
using Asp.Versioning;
using Domain.Models.Identity;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var apiVersion = new ApiVersion(1);
const string apiVersionGroupNameFormat = "'v'V";

var builder = WebApplication.CreateBuilder(args);

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

if (builder.Environment.IsProduction()) // TODO Pawel: IsProductionOrTest()?
{
	builder.Services.AddAntiforgery();
}

builder.Services.AddAuthentication();
builder.Services.AddAuthorizationBuilder()
	.AddPolicies();

builder.Services
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

await app.Services.InitializeDb(builder.Configuration).ConfigureAwait(false);

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

if (app.Environment.IsProduction()) // TODO Pawel: IsProductionOrTest()?
{
	app.Strip404Body();
	app.Use404InsteadOf403();
	app.UseAntiforgery();
}

app.UseHttpsRedirection();

await app.Services.AddRoles().ConfigureAwait(false);

app.MapAllRoutes();

app.UseStaticFiles();

app.Run();

public partial class Program; // for functional tests
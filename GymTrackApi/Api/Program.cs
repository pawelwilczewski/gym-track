using Api.Authorization;
using Api.Common;
using Api.Routes;
using Application.Serialization;
using Asp.Versioning;
using Domain.Models.Identity;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddEndpointsApiExplorer();

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddSwaggerGen();
}
else
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
		options.DefaultApiVersion = new ApiVersion(1);
		options.ApiVersionReader = new UrlSegmentApiVersionReader();
	})
	.AddApiExplorer(options =>
	{
		options.GroupNameFormat = "'v'V";
		options.SubstituteApiVersionInUrl = true;
	});

builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.Configure());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.Services.ApplyMigrations();

	app.UseSwagger();
	app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
	app.Use404InsteadOf403();
	app.Strip404Body();
	app.UseAntiforgery();
}

app.UseHttpsRedirection();

await app.Services.AddRoles();

app.MapAllRoutes();

app.UseStaticFiles();

app.Run();
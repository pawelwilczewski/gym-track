using Api.Authorization;
using Api.Routes;
using Application.Serialization;
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

builder.Services.AddAuthentication();
builder.Services.AddAuthorizationBuilder()
	.AddPolicies();

builder.Services
	.AddInfrastructureDependencies(builder.Configuration);

builder.Services
	.AddIdentityApiEndpoints<User>()
	.AddDefaultTokenProviders();

builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.Configure());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.Services.ApplyMigrations();

	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.Services.AddRoles();

app.MapAllRoutes();

app.Run();
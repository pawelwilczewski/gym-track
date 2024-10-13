using Api.Routes.Identity;
using Api.Routes.Workout;
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
builder.Services.AddAuthorization();

builder.Services
	.AddInfrastructureDependencies(builder.Configuration);

builder.Services
	.AddIdentityApiEndpoints<User>()
	.AddDefaultTokenProviders();

builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.Configure());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();

	app.Services.ApplyMigrations();
}

app.UseHttpsRedirection();

app
	.MapIdentityRoutes()
	.MapWorkoutRoutes();

app.Run();
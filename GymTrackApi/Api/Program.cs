using Api.Routes;
using Domain.Models.User;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddEndpointsApiExplorer();

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddSwaggerGen();
}

builder.Services
	.AddAuthorization()
	.AddAuthentication();

builder.Services
	.AddInfrastructureDependencies(builder.Configuration);

builder.Services
	.AddIdentityApiEndpoints<AppUser>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();

	app.Services.ApplyMigrations();
}

app.UseHttpsRedirection();

app.MapIdentityApi<AppUser>();
app.MapUsers();

app.Run();
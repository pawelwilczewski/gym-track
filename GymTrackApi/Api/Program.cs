using Api.Routes;
using Domain.Models.Identity;
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

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services
	.AddInfrastructureDependencies(builder.Configuration);

builder.Services
	.AddIdentityApiEndpoints<User>()
	.AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();

	app.Services.ApplyMigrations();
}

app.UseHttpsRedirection();

app.MapIdentityApi();
app.MapUsers();

app.Run();
using Api.Common;
using Api.Files;
using Api.Middleware;
using Api.Routes;
using Application;
using Application.Persistence;
using Asp.Versioning;
using Infrastructure;
using Infrastructure.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
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
	builder.Services.AddSwaggerGen(options =>
	{
		options.SwaggerDoc("v1", new OpenApiInfo
		{
			Title = "Gym Track API",
			Version = apiVersion.ToString(apiVersionGroupNameFormat)
		});

		var jwtSecurityScheme = new OpenApiSecurityScheme
		{
			BearerFormat = "JWT",
			Name = "JWT Authentication",
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.Http,
			Scheme = JwtBearerDefaults.AuthenticationScheme,
			Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

			Reference = new OpenApiReference
			{
				Id = JwtBearerDefaults.AuthenticationScheme,
				Type = ReferenceType.SecurityScheme
			}
		};

		options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

		options.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{ jwtSecurityScheme, Array.Empty<string>() }
		});
	});
}

builder.Services.AddAntiforgery(options =>
{
	options.FormFieldName = "__RequestVerificationToken";
	options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services
	.AddApplicationDependencies()
	.AddInfrastructureDependencies(builder.Configuration);

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

await app.Services.ConfigureAppInfrastructure().ConfigureAwait(false);

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

app.UseAntiforgery();

app.AddPutFormSupport();

app.UseAuthentication();
app.UseAuthorization();

app.MapAllRoutes();

app.Run();

public partial class Program; // for functional tests
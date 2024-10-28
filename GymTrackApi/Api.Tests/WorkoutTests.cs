using System.Security.Claims;
using Api.Dtos;
using Api.Routes.App.Workouts;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests;

internal sealed class WorkoutTests
{
	private static HttpContext httpContext = default!;

	[Before(Class)]
	public static void SetUpAll()
	{
		Claim[] identityClaims = [new(ClaimTypes.Role, Role.ADMINISTRATOR)];
		var userClaims = new ClaimsPrincipal(new ClaimsIdentity(identityClaims));
		httpContext = new DefaultHttpContext
		{
			User = userClaims
		};
	}

	private IDataContext dataContext = default!;

	[Before(Test)]
	public async Task SetUpEach()
	{
		dataContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase("GymTrack-Test")
			.Options);

		Name.TryCreate("Hello", out var name, out _);
		await dataContext.Workouts.AddAsync(Workout.CreateForEveryone(name!));
	}

	[After(Test)]
	public void CleanUpEach()
	{
		dataContext.Dispose();
	}

	[Test]
	public async Task CreateWorkout_AdminWithValidData_ReturnsOk()
	{
		var result = await CreateWorkout.Handler(
			httpContext,
			new CreateWorkoutRequest("Test Workout"),
			dataContext,
			CancellationToken.None
		);

		await Assert.That(result.Result).IsTypeOf<Ok>();
	}
}
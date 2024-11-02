using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Tests.Mocks;
using Domain.Models;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class WorkoutTests
{
	[Test]
	public async Task CreateWorkout_AdminWithValidData_ReturnsSuccess()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser("test@test.com")
			.WithAdminUser("admin@admin.com")
			.Build()
			.ConfigureAwait(false);

		Name.TryCreate("Hello", out var name, out _);
		await dataContext.Workouts.AddAsync(Workout.CreateForEveryone(name!)).ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				HttpContextMocks.Admin,
				new CreateWorkoutRequest("Admin's Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Created>();
	}

	[Test]
	public async Task CreateWorkout_UserWithValidData_ReturnsSuccess()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser("test@test.com")
			.WithAdminUser("admin@admin.com")
			.Build()
			.ConfigureAwait(false);

		Name.TryCreate("Hello", out var name, out _);
		await dataContext.Workouts.AddAsync(Workout.CreateForEveryone(name!)).ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				HttpContextMocks.User,
				new CreateWorkoutRequest("User's Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Created>();
	}
}
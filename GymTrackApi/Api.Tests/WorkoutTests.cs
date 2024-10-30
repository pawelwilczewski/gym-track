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
	public async Task CreateWorkout_AdminWithValidData_ReturnsOk()
	{
		using var dataContext = await (await MockDataContextBuilder.CreateEmpty())
			.WithUser("test@test.com")
			.WithAdminUser("admin@admin.com")
			.Build()
			.ConfigureAwait(false);

		Name.TryCreate("Hello", out var name, out _);
		await dataContext.Workouts.AddAsync(Workout.CreateForEveryone(name!)).ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				HttpContextMocks.Admin,
				new CreateWorkoutRequest("Test Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Ok>();
	}
}
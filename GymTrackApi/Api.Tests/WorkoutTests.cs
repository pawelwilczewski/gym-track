using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Tests.Mocks;
using Domain.Models;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class WorkoutTests : DataContextBasedTests
{
	[Test]
	public async Task CreateWorkout_AdminWithValidData_ReturnsOk()
	{
		Name.TryCreate("Hello", out var name, out _);
		await DataContext.Workouts.AddAsync(Workout.CreateForEveryone(name!));

		var result = await CreateWorkout.Handler(
			HttpContextMocks.Admin,
			new CreateWorkoutRequest("Test Workout"),
			DataContext,
			CancellationToken.None);

		await Assert.That(result.Result).IsTypeOf<Ok>();
	}
}
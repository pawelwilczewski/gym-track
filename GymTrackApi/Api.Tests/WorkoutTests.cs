using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Tests.Mocks;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class WorkoutTests
{
	private IDataContext dataContext = default!;

	[Before(Test)]
	public async Task SetUpEach()
	{
		dataContext = DataContextMocks.CreateEmpty();

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
			HttpContextMocks.Admin,
			new CreateWorkoutRequest("Test Workout"),
			dataContext,
			CancellationToken.None
		);

		await Assert.That(result.Result).IsTypeOf<Ok>();
	}
}
using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Tests.Mocks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class WorkoutTests
{
	[Test]
	public async Task CreateWorkout_AdminWithValidData_ReturnsSuccess()
	{
		var adminInfo = new UserInfo(Guid.NewGuid(), "admin@admin.com", "Password@123");

		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAdminUser(adminInfo)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				HttpContextMocks.ForAdmin(adminInfo),
				new CreateWorkoutRequest("Admin's Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Created>();
	}

	[Test]
	public async Task CreateWorkout_UserWithValidData_ReturnsSuccess()
	{
		var userInfo = new UserInfo(Guid.NewGuid(), "user@user.com", "Password@123");

		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(userInfo)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				HttpContextMocks.ForUser(userInfo),
				new CreateWorkoutRequest("User's Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Created>();
	}
}
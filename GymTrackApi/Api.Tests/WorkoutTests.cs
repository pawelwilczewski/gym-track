using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class WorkoutTests
{
	private static readonly UserInfo admin = new(Guid.NewGuid(), "admin@admin.com", "Password@123");
	private static readonly HttpContext httpContextAdmin = HttpContextMocks.ForAdmin(admin);

	private static readonly UserInfo user1 = new(Guid.NewGuid(), "user1@user.com", "Password@123");
	private static readonly HttpContext httpContextUser1 = HttpContextMocks.ForUser(user1);

	private static readonly UserInfo user2 = new(Guid.NewGuid(), "user2@user.com", "Password@123");
	private static readonly HttpContext httpContextUser2 = HttpContextMocks.ForUser(user2);

	[Test]
	public async Task CreateWorkout_AdminWithValidData_ReturnsCreated()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAdminUser(admin)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				httpContextAdmin,
				new CreateWorkoutRequest("Admin's Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Created>();
	}

	[Test]
	public async Task CreateWorkout_UserWithValidData_ReturnsCreated()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(user1)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				httpContextUser1,
				new CreateWorkoutRequest("User's Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Created>();
	}

	[Test]
	public async Task GetWorkout_UserAccessShared_ReturnsOk()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(user1)
			.WithWorkout(out var workout)
			.Build()
			.ConfigureAwait(false);

		var result = await GetWorkout.Handler(
				httpContextUser1,
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Ok<GetWorkoutResponse>>();
	}

	[Test]
	public async Task GetWorkout_UserAccessUsers_ReturnsOk()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(user1)
			.WithWorkout(out var workout, httpContextUser1.User)
			.Build()
			.ConfigureAwait(false);

		var result = await GetWorkout.Handler(
				httpContextUser1,
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Ok<GetWorkoutResponse>>();
	}

	[Test]
	public async Task GetWorkout_UserAccessAnotherUsers_ReturnsForbid()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(user1)
			.WithUser(user2)
			.WithWorkout(out var workout, httpContextUser1.User)
			.Build()
			.ConfigureAwait(false);

		var result = await GetWorkout.Handler(
				httpContextUser2,
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<ForbidHttpResult>();
	}
}
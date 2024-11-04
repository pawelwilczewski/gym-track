using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Tests.Mocks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class WorkoutTests
{
	[Test]
	public async Task CreateWorkout_AdminWithValidData_ReturnsCreated()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAdmin(Users.Admin1)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				Users.Admin1.GetHttpContext(),
				new CreateWorkoutRequest("Admin's Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Created>();
	}

	[Test]
	public async Task CreateWorkout_AdminWithInvalidData_ReturnsValidationProblem()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAdmin(Users.Admin1)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				Users.Admin1.GetHttpContext(),
				new CreateWorkoutRequest(""),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<ValidationProblem>();

		result = await CreateWorkout.Handler(
				Users.Admin1.GetHttpContext(),
				new CreateWorkoutRequest(null!),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<ValidationProblem>();
	}

	[Test]
	public async Task CreateWorkout_UserWithValidData_ReturnsCreated()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				Users.User1.GetHttpContext(),
				new CreateWorkoutRequest("User's Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Created>();
	}

	[Test]
	public async Task CreateWorkout_UserWithInvalidData_ReturnsValidationProblem()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				Users.User1.GetHttpContext(),
				new CreateWorkoutRequest(""),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<ValidationProblem>();

		result = await CreateWorkout.Handler(
				Users.User1.GetHttpContext(),
				new CreateWorkoutRequest(null!),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<ValidationProblem>();
	}

	[Test]
	public async Task GetWorkout_UserAccessShared_ReturnsOk()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.WithWorkout(out var workout)
			.Build()
			.ConfigureAwait(false);

		var result = await GetWorkout.Handler(
				Users.User1.GetHttpContext(),
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<Ok<GetWorkoutResponse>>();
	}

	[Test]
	public async Task GetWorkout_UserAccessTheirOwn_ReturnsOk()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.WithWorkout(out var workout, Users.User1.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await GetWorkout.Handler(
				Users.User1.GetHttpContext(),
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
			.WithUser(Users.User1)
			.WithUser(Users.User2)
			.WithWorkout(out var workout, Users.User1.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await GetWorkout.Handler(
				Users.User2.GetHttpContext(),
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<ForbidHttpResult>();
	}

	[Test]
	public async Task EditWorkout_AdminWithValidData_ReturnsNoContent()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAdmin(Users.Admin1)
			.WithWorkout(out var workout)
			.Build()
			.ConfigureAwait(false);

		var result = await EditWorkout.Handler(
				Users.Admin1.GetHttpContext(),
				workout.Id.Value,
				new EditWorkoutRequest("Updated Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<NoContent>();
	}

	[Test]
	public async Task EditWorkout_UserWithValidData_ReturnsNoContent()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.WithWorkout(out var workout, Users.User1.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await EditWorkout.Handler(
				Users.User1.GetHttpContext(),
				workout.Id.Value,
				new EditWorkoutRequest("Updated Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<NoContent>();
	}

	[Test]
	public async Task EditWorkout_UserSomeoneElsesWithValidData_ReturnsForbid()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.WithUser(Users.User2)
			.WithWorkout(out var workout, Users.User1.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await EditWorkout.Handler(
				Users.User2.GetHttpContext(),
				workout.Id.Value,
				new EditWorkoutRequest("Updated Workout"),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<ForbidHttpResult>();
	}

	[Test]
	public async Task EditWorkout_UserWithInvalidData_ReturnsValidationProblem()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.WithWorkout(out var workout, Users.User1.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await EditWorkout.Handler(
				Users.User1.GetHttpContext(),
				workout.Id.Value,
				new EditWorkoutRequest(""),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<ValidationProblem>();
	}

	[Test]
	public async Task DeleteWorkout_Admin_ReturnsNoContent()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAdmin(Users.Admin1)
			.WithWorkout(out var workout)
			.Build()
			.ConfigureAwait(false);

		var result = await DeleteWorkout.Handler(
				Users.Admin1.GetHttpContext(),
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<NoContent>();
	}

	[Test]
	public async Task DeleteWorkout_User_ReturnsNoContent()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.WithWorkout(out var workout, Users.User1.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await DeleteWorkout.Handler(
				Users.User1.GetHttpContext(),
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<NoContent>();
	}

	[Test]
	public async Task DeleteWorkout_UserAccessAnotherUsers_ReturnsForbid()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.WithUser(Users.User2)
			.WithWorkout(out var workout, Users.User1.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await DeleteWorkout.Handler(
				Users.User2.GetHttpContext(),
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf<ForbidHttpResult>();
	}
}
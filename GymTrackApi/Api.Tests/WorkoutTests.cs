using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Tests.Mocks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class WorkoutTests
{
	public static IEnumerable<(IUserInfo userInfo, string workoutName, Type responseType)> CreateWorkoutData()
	{
		List<IUserInfo> users = [Users.Admin1, Users.User1];
		List<(string name, Type responseType)> workoutNames =
		[
			new("ABC Workout", typeof(Created)),
			new("A", typeof(Created)),
			new(",.;-", typeof(ValidationProblem)),
			new(null!, typeof(ValidationProblem)),
			new("", typeof(ValidationProblem))
		];

		return users.SelectMany(user => workoutNames.Select(tuple => (user, tuple.name, tuple.responseType)));
	}

	public static IEnumerable<(IUserInfo? owner, IUserInfo accessor, Type responseType)> GetWorkoutData() =>
	[
		new(null, Users.User1, typeof(Ok<GetWorkoutResponse>)),
		new(Users.User1, Users.User1, typeof(Ok<GetWorkoutResponse>)),
		new(Users.User2, Users.User1, typeof(ForbidHttpResult))
	];

	public static IEnumerable<(IUserInfo? owner, IUserInfo editor, string workoutName, Type responseType)> EditWorkoutData() =>
	[
		new(null, Users.User1, "ValidName", typeof(ForbidHttpResult)),
		new(null, Users.Admin1, "ValidName", typeof(NoContent)),
		new(Users.User1, Users.User1, "ValidName", typeof(NoContent)),
		new(Users.User2, Users.User1, "ValidName", typeof(ForbidHttpResult))
	];

	public static IEnumerable<(IUserInfo? owner, IUserInfo deleter, Type responseType)> DeleteWorkoutData() =>
	[
		new(null, Users.User1, typeof(ForbidHttpResult)),
		new(null, Users.Admin1, typeof(NoContent)),       // TODO: or forbid if other users reference it?
		new(Users.User1, Users.User1, typeof(NoContent)), // TODO: or forbid if other users reference it?
		new(Users.User2, Users.User1, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(CreateWorkoutData))]
	public async Task CreateWorkout_ReturnsCorrectResponse(IUserInfo user, string workoutName, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.Admin1)
			.WithUser(Users.User1)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkout.Handler(
				user.GetHttpContext(),
				new CreateWorkoutRequest(workoutName),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	[Test]
	[MethodDataSource(nameof(GetWorkoutData))]
	public async Task GetWorkout_ReturnsCorrectResponse(IUserInfo? workoutOwner, IUserInfo accessor, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.WithUser(Users.User2)
			.WithWorkout(out var workout, workoutOwner?.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await GetWorkout.Handler(
				accessor.GetHttpContext(),
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	[Test]
	[MethodDataSource(nameof(EditWorkoutData))]
	public async Task EditWorkout_ReturnsCorrectResponse(IUserInfo? workoutOwner, IUserInfo editor, string workoutName, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.Admin1)
			.WithUser(Users.User1)
			.WithWorkout(out var workout, workoutOwner?.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await EditWorkout.Handler(
				editor.GetHttpContext(),
				workout.Id.Value,
				new EditWorkoutRequest(workoutName),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	[Test]
	[MethodDataSource(nameof(DeleteWorkoutData))]
	public async Task DeleteWorkout_ReturnsCorrectResponse(IUserInfo? workoutOwner, IUserInfo deleter, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.Admin1)
			.WithWorkout(out var workout, workoutOwner?.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await DeleteWorkout.Handler(
				deleter.GetHttpContext(),
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}
}
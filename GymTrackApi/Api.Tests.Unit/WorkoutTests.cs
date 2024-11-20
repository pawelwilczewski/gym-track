using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Tests.Unit.Mocks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests.Unit;

internal sealed class WorkoutTests
{
	public static IEnumerable<(IUserInfo creator, string workoutName, Type responseType)> CreateWorkoutData()
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

		foreach (var user in users)
		{
			foreach (var (name, responseType) in workoutNames)
			{
				yield return new ValueTuple<IUserInfo, string, Type>(user, name, responseType);
			}
		}
	}

	[Test]
	[MethodDataSource(nameof(CreateWorkoutData))]
	public async Task CreateWorkout_ReturnsCorrectResponse(IUserInfo user, string workoutName, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
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

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo accessor, Type responseType)> GetWorkoutData() =>
	[
		new([Users.Admin1], Users.User1, typeof(Ok<GetWorkoutResponse>)),
		new([Users.User1], Users.Admin1, typeof(Ok<GetWorkoutResponse>)),
		new([Users.User2], Users.Admin1, typeof(Ok<GetWorkoutResponse>)),
		new([Users.User1], Users.User1, typeof(Ok<GetWorkoutResponse>)),
		new([Users.User2], Users.User1, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(GetWorkoutData))]
	public async Task GetWorkout_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo accessor, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owners)
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

	public static IEnumerable<(IReadOnlyList<IReadOnlyList<IUserInfo>> workoutsOwners, IUserInfo accessor, int returnedCount)> GetWorkoutsData() =>
	[
		new([[Users.Admin1], [Users.User1]], Users.User1, 2),
		new([[Users.User1], [Users.User2], [Users.Admin1]], Users.User1, 2),
		new([[Users.User2], [Users.User2], [Users.User2]], Users.User1, 0),
		new([], Users.User1, 0),
		new([[Users.Admin1], [Users.Admin1], [Users.User2]], Users.User2, 3)
	];

	[Test]
	[MethodDataSource(nameof(GetWorkoutsData))]
	public async Task GetWorkouts_ReturnsCorrectCount(IReadOnlyList<IReadOnlyList<IUserInfo>> workoutsOwners, IUserInfo accessor, int returnedCount)
	{
		var builder = MockDataContextBuilder.CreateEmpty()
			.WithAllUsers();

		foreach (var owners in workoutsOwners)
		{
			builder.WithWorkout(out _, owners);
		}

		using var dataContext = await builder
			.Build()
			.ConfigureAwait(false);

		var result = await GetWorkouts.Handler(
				accessor.GetHttpContext(),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(typeof(Ok<List<GetWorkoutResponse>>));

		await Assert.That(((Ok<List<GetWorkoutResponse>>)result.Result).Value?.Count).IsEqualTo(returnedCount);
	}

	[Test]
	public async Task GetWorkout_InvalidId_ReturnsNotFound()
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out _, [])
			.Build()
			.ConfigureAwait(false);

		var result = await GetWorkout.Handler(
				Users.User1.GetHttpContext(),
				new Guid(),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(typeof(NotFound<string>));
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo editor, string workoutName, Type responseType)> EditWorkoutData() =>
	[
		new([Users.Admin1], Users.User1, "ValidName", typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, "ValidName", typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, "ValidName", typeof(NoContent)),
		new([Users.User1], Users.User1, "ValidName", typeof(NoContent)),
		new([Users.User1, Users.User2], Users.User1, "ValidName", typeof(ForbidHttpResult)),
		new([Users.User2], Users.User1, "ValidName", typeof(ForbidHttpResult)),
		new([Users.User2, Users.User1], Users.User1, "ValidName", typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(EditWorkoutData))]
	public async Task EditWorkout_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo editor, string workoutName, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owners)
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

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo deleter, Type responseType)> DeleteWorkoutData() =>
	[
		new([Users.Admin1], Users.User1, typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, typeof(NoContent)),
		new([Users.User1], Users.User1, typeof(NoContent)),
		new([Users.User1, Users.User2], Users.User1, typeof(ForbidHttpResult)),
		new([Users.User2], Users.User1, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(DeleteWorkoutData))]
	public async Task DeleteWorkout_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo deleter, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owners)
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
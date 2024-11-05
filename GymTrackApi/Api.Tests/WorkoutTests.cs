using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Tests.Mocks;
using Domain.Models.Workout;
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

	public static IEnumerable<(IUserInfo owner, IUserInfo accessor, Type responseType)> GetWorkoutData() =>
	[
		new(Users.Admin1, Users.User1, typeof(Ok<GetWorkoutResponse>)),
		new(Users.User1, Users.User1, typeof(Ok<GetWorkoutResponse>)),
		new(Users.User2, Users.User1, typeof(ForbidHttpResult))
	];

	public static IEnumerable<(IUserInfo owner, IUserInfo editor, string workoutName, Type responseType, Action<Workout>? configureWorkout)> EditWorkoutData() =>
	[
		new(Users.Admin1, Users.User1, "ValidName", typeof(ForbidHttpResult), null),
		new(Users.Admin1, Users.Admin1, "ValidName", typeof(NoContent), null),
		new(Users.Admin1, Users.Admin1, "ValidName", typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User2.Id, workout.Id))),
		new(Users.User1, Users.User1, "ValidName", typeof(NoContent), null),
		new(Users.User1, Users.User1, "ValidName", typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User2.Id, workout.Id))),
		new(Users.User2, Users.User1, "ValidName", typeof(ForbidHttpResult), null),
		new(Users.User2, Users.User1, "ValidName", typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User1.Id, workout.Id)))
	];

	public static IEnumerable<(IUserInfo owner, IUserInfo deleter, Type responseType, Action<Workout>? configureWorkout)> DeleteWorkoutData() =>
	[
		new(Users.Admin1, Users.User1, typeof(ForbidHttpResult), null),
		new(Users.Admin1, Users.Admin1, typeof(NoContent), null),
		new(Users.Admin1, Users.Admin1, typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User2.Id, workout.Id))),
		new(Users.User1, Users.User1, typeof(NoContent), null),
		new(Users.User1, Users.User1, typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User2.Id, workout.Id))),
		new(Users.User2, Users.User1, typeof(ForbidHttpResult), null)
	];

	[Test]
	[MethodDataSource(nameof(CreateWorkoutData))]
	public async Task CreateWorkout_ReturnsCorrectResponse(IUserInfo user, string workoutName, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(user)
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
	public async Task GetWorkout_ReturnsCorrectResponse(IUserInfo workoutOwner, IUserInfo accessor, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(workoutOwner)
			.WithUser(accessor)
			.WithWorkout(out var workout, workoutOwner.GetHttpContext().User)
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
	public async Task EditWorkout_ReturnsCorrectResponse(IUserInfo workoutOwner, IUserInfo editor, string workoutName, Type responseType, Action<Workout>? configureWorkout = null)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.Admin1)
			.WithUser(Users.User1)
			.WithUser(Users.User2)
			.WithWorkout(out var workout, workoutOwner.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		if (configureWorkout is not null)
		{
			configureWorkout(workout);
			await dataContext.SaveChangesAsync(default).ConfigureAwait(false);
		}

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
	public async Task DeleteWorkout_ReturnsCorrectResponse(IUserInfo workoutOwner, IUserInfo deleter, Type responseType, Action<Workout>? configureWorkout = null)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.Admin1)
			.WithUser(Users.User1)
			.WithUser(Users.User2)
			.WithWorkout(out var workout, workoutOwner.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		if (configureWorkout is not null)
		{
			configureWorkout(workout);
			await dataContext.SaveChangesAsync(default).ConfigureAwait(false);
		}

		var result = await DeleteWorkout.Handler(
				deleter.GetHttpContext(),
				workout.Id.Value,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}
}
using Application.Tests.Unit.Mocks;
using Application.Workout.Commands;
using Application.Workout.Dtos;
using Application.Workout.Queries;
using Domain.Common.ValueObjects;
using Domain.Models;
using Infrastructure.Persistence;
using OneOf.Types;

namespace Application.Tests.Unit;

internal sealed class WorkoutTests
{
	public static IEnumerable<(IUserInfo creator, Name workoutName, Type responseType)> CreateWorkoutData()
	{
		List<IUserInfo> users = [Users.Admin1, Users.User1];
		List<(string name, Type responseType)> workoutNames =
		[
			new("ABC Workout", typeof(Success<GetWorkoutResponse>)),
			new("A", typeof(Success<GetWorkoutResponse>))
		];

		foreach (var user in users)
		{
			foreach (var (name, responseType) in workoutNames)
			{
				yield return new ValueTuple<IUserInfo, Name, Type>(user, Name.From(name), responseType);
			}
		}
	}

	[Test]
	[MethodDataSource(nameof(CreateWorkoutData))]
	public async Task CreateWorkout_ReturnsCorrectResponse(IUserInfo user, Name workoutName, Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.Build()
			.ConfigureAwait(false);

		var handler = new CreateWorkoutHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
				new CreateWorkoutCommand(workoutName, user.Id), CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo accessor, Type responseType)> GetWorkoutData() =>
	[
		new(Users.Admin1, Users.User1, typeof(NotFound)),
		new(Users.User1, Users.Admin1, typeof(NotFound)),
		new(Users.User2, Users.Admin1, typeof(NotFound)),
		new(Users.User1, Users.User1, typeof(Success<GetWorkoutResponse>)),
		new(Users.User2, Users.User1, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(GetWorkoutData))]
	public async Task GetWorkout_ReturnsCorrectResponse(IUserInfo owner, IUserInfo accessor, Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owner)
			.Build()
			.ConfigureAwait(false);

		var handler = new GetWorkoutHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
				new GetWorkoutQuery(workout.Id, accessor.Id), CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> workoutsOwners, IUserInfo accessor, int returnedCount)> GetWorkoutsData() =>
	[
		new([Users.Admin1, Users.User1], Users.User1, 1),
		new([Users.User1, Users.User2, Users.Admin1], Users.User1, 1),
		new([Users.User2, Users.User2, Users.User2], Users.User1, 0),
		new([], Users.User1, 0),
		new([Users.Admin1, Users.Admin1, Users.User2], Users.User2, 1)
	];

	[Test]
	[MethodDataSource(nameof(GetWorkoutsData))]
	public async Task GetWorkouts_ReturnsCorrectCount(IReadOnlyList<IUserInfo> workoutsOwners, IUserInfo accessor, int returnedCount)
	{
		var builder = MockDataContextBuilder.CreateEmpty()
			.WithAllUsers();

		foreach (var owner in workoutsOwners)
		{
			builder.WithWorkout(out _, owner);
		}

		await using var dataContext = await builder
			.Build()
			.ConfigureAwait(false);

		var handler = new GetWorkoutsHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
				new GetWorkoutsQuery(accessor.Id), CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Value.Count).IsEqualTo(returnedCount);
	}

	[Test]
	public async Task GetWorkout_InvalidId_ReturnsNotFound()
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out _, Users.User1)
			.Build()
			.ConfigureAwait(false);

		var handler = new GetWorkoutHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
				new GetWorkoutQuery(new Id<Domain.Models.Workout.Workout>(new Guid()), Users.User1.Id), CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Value).IsTypeOf(typeof(NotFound));
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo editor, Name workoutName, Type responseType)> UpdateWorkoutData() =>
	[
		new(Users.Admin1, Users.User1, Name.From("ValidName"), typeof(NotFound)),
		new(Users.Admin1, Users.Admin1, Name.From("ValidName"), typeof(Success)),
		new(Users.Admin1, Users.Admin1, Name.From("ValidName"), typeof(Success)),
		new(Users.User1, Users.User1, Name.From("ValidName"), typeof(Success)),
		new(Users.User2, Users.User1, Name.From("ValidName"), typeof(NotFound)),
		new(Users.User2, Users.User1, Name.From("ValidName"), typeof(NotFound)),
		new(Users.User2, Users.User1, Name.From("ValidName"), typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(UpdateWorkoutData))]
	public async Task UpdateWorkout_ReturnsCorrectResponse(IUserInfo owner, IUserInfo editor, Name workoutName, Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owner)
			.Build()
			.ConfigureAwait(false);

		var handler = new UpdateWorkoutHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
				new UpdateWorkoutCommand(workout.Id, workoutName, editor.Id), CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo deleter, Type responseType)> DeleteWorkoutData() =>
	[
		new(Users.Admin1, Users.User1, typeof(NotFound)),
		new(Users.Admin1, Users.Admin1, typeof(Success)),
		new(Users.User1, Users.User1, typeof(Success)),
		new(Users.User2, Users.User1, typeof(NotFound)),
		new(Users.User2, Users.User1, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(DeleteWorkoutData))]
	public async Task DeleteWorkout_ReturnsCorrectResponse(IUserInfo owner, IUserInfo deleter, Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owner)
			.Build()
			.ConfigureAwait(false);

		var handler = new DeleteWorkoutHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
				new DeleteWorkoutCommand(workout.Id, deleter.Id), CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}
}
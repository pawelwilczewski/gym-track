using Api.Dtos;
using Api.Routes.App.Workouts;
using Api.Routes.App.Workouts.Exercises;
using Api.Tests.Mocks;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class WorkoutExerciseTests
{
	public static IEnumerable<(IUserInfo creator, IUserInfo exerciseInfoOwner, Type responseType)> CreateWorkoutExerciseData() =>
	[
		new(Users.Admin1, Users.Admin1, typeof(Created)),
		new(Users.Admin1, Users.User1, typeof(Created)),
		new(Users.User1, Users.Admin1, typeof(Created)),
		new(Users.User2, Users.User1, typeof(ForbidHttpResult))
	];

	public static IEnumerable<(IUserInfo owner, IUserInfo accessor, Type responseType)> GetWorkoutExerciseData() =>
	[
		new(Users.Admin1, Users.User1, typeof(Ok<GetWorkoutResponse>)),
		new(Users.User1, Users.User1, typeof(Ok<GetWorkoutResponse>)),
		new(Users.User2, Users.User1, typeof(ForbidHttpResult)),
		new(Users.User1, Users.Admin1, typeof(Ok<GetWorkoutResponse>))
	];

	[Test]
	[MethodDataSource(nameof(CreateWorkoutExerciseData))]
	public async Task CreateWorkoutExercise_ReturnsCorrectResponse(IUserInfo creator, IUserInfo? exerciseInfoOwner, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.Admin1)
			.WithUser(Users.User1)
			.WithUser(Users.User2)
			.WithWorkout(out var workout, creator.GetHttpContext().User)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, exerciseInfoOwner?.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkoutExercise.Handler(
				creator.GetHttpContext(),
				workout.Id.Value,
				new CreateWorkoutExerciseRequest(0, exerciseInfo.Id.Value),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	[Test]
	[MethodDataSource(nameof(GetWorkoutExerciseData))]
	public async Task GetWorkoutExercise_ReturnsCorrectResponse(IUserInfo workoutOwner, IUserInfo accessor, Type responseType)
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
}
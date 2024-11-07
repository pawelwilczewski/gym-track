using Api.Dtos;
using Api.Routes.App.Workouts.Exercises;
using Api.Tests.Mocks;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Index = Domain.Models.Index;

namespace Api.Tests;

internal sealed class WorkoutExerciseTests
{
	public static IEnumerable<(IUserInfo creator, IUserInfo exerciseInfoOwner, int exerciseIndex, Type responseType)> CreateWorkoutExerciseData() =>
	[
		new(Users.Admin1, Users.Admin1, 0, typeof(Created)),
		new(Users.Admin1, Users.User1, -1, typeof(ValidationProblem)),
		new(Users.User1, Users.Admin1, 0, typeof(Created)),
		new(Users.User2, Users.User1, 0, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(CreateWorkoutExerciseData))]
	public async Task CreateWorkoutExercise_ReturnsCorrectResponse(IUserInfo creator, IUserInfo exerciseInfoOwner, int exerciseIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, [creator])
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, [exerciseInfoOwner])
			.Build()
			.ConfigureAwait(false);

		var result = await CreateWorkoutExercise.Handler(
				creator.GetHttpContext(),
				workout.Id.Value,
				new CreateWorkoutExerciseRequest(exerciseIndex, exerciseInfo.Id.Value),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo accessor, int exerciseIndex, int accessedExerciseIndex, Type responseType)> GetWorkoutExerciseData() =>
	[
		new([Users.Admin1], Users.User1, 0, 0, typeof(Ok<GetWorkoutExerciseResponse>)),
		new([Users.Admin1], Users.User1, 0, 1, typeof(NotFound<string>)),
		new([Users.User1], Users.User1, 0, 0, typeof(Ok<GetWorkoutExerciseResponse>)),
		new([Users.User1], Users.User1, 1, 1, typeof(Ok<GetWorkoutExerciseResponse>)),
		new([Users.User2], Users.User1, 0, 0, typeof(ForbidHttpResult)),
		new([Users.User1], Users.Admin1, 0, -1, typeof(NotFound<string>)),
		new([Users.User1], Users.Admin1, 0, 0, typeof(Ok<GetWorkoutExerciseResponse>))
	];

	[Test]
	[MethodDataSource(nameof(GetWorkoutExerciseData))]
	public async Task GetWorkoutExercise_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo accessor, int exerciseIndex, int accessedExerciseIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwners)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, workoutOwners)
			.Build()
			.ConfigureAwait(false);

		if (!Index.TryCreate(exerciseIndex, out var index)) throw new Exception("Invalid test case");

		workout.Exercises.Add(new Workout.Exercise(workout.Id, index, exerciseInfo.Id));
		await dataContext.SaveChangesAsync(default).ConfigureAwait(false);

		var result = await GetWorkoutExercise.Handler(
				accessor.GetHttpContext(),
				workout.Id.Value,
				accessedExerciseIndex,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo deleter, int exerciseIndex, int deletedExerciseIndex, Type responseType)> DeleteWorkoutExerciseData() =>
	[
		new([Users.Admin1], Users.User1, 0, 0, typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, 0, 0, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, 0, 0, typeof(NoContent)),
		new([Users.User1], Users.User1, 0, 0, typeof(NoContent)),
		new([Users.User1, Users.User2], Users.User1, 0, 0, typeof(ForbidHttpResult)),
		new([Users.User2], Users.User1, 0, 0, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(DeleteWorkoutExerciseData))]
	public async Task DeleteWorkoutExercise_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo deleter, int exerciseIndex, int deletedExerciseIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwners)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, workoutOwners)
			.Build()
			.ConfigureAwait(false);

		if (!Index.TryCreate(exerciseIndex, out var index)) throw new Exception("Invalid test case");

		workout.Exercises.Add(new Workout.Exercise(workout.Id, index, exerciseInfo.Id));
		await dataContext.SaveChangesAsync(default).ConfigureAwait(false);

		var result = await DeleteWorkoutExercise.Handler(
				deleter.GetHttpContext(),
				workout.Id.Value,
				deletedExerciseIndex,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}
}
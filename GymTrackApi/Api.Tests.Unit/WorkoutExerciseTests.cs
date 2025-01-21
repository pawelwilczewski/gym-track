using Api.Dtos;
using Api.Routes.App.Workouts.Exercises;
using Api.Routes.App.Workouts.Exercises.DisplayOrder;
using Api.Tests.Unit.Mocks;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests.Unit;

internal sealed class WorkoutExerciseTests
{
	public static IEnumerable<(IUserInfo creator, IUserInfo exerciseInfoOwner, int exerciseIndex, Type responseType)> CreateWorkoutExerciseData() =>
	[
		new(Users.Admin1, Users.Admin1, 0, typeof(Created)),
		new(Users.Admin1, Users.User1, -1, typeof(Created)),
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
				new CreateWorkoutExerciseRequest(exerciseInfo.Id.Value),
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

		workout.Exercises.Add(new Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0));
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

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo editor, int displayOrder, Type responseType)> EditWorkoutExerciseDisplayOrderData() =>
	[
		new([Users.Admin1], Users.Admin1, 4, typeof(NoContent)),
		new([Users.Admin1], Users.User1, 6, typeof(ForbidHttpResult)),
		new([Users.User1], Users.User1, 2, typeof(NoContent)),
		new([Users.User1], Users.User1, 7, typeof(NoContent))
	];

	[Test]
	[MethodDataSource(nameof(EditWorkoutExerciseDisplayOrderData))]
	public async Task EditWorkoutExerciseDisplayOrder_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo editor, int displayOrder, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owners)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.All, owners)
			.Build()
			.ConfigureAwait(false);

		const int exerciseIndex = 0;

		var exercise = new Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.Exercises.Add(exercise);
		await dataContext.SaveChangesAsync(default).ConfigureAwait(false);

		var result = await EditWorkoutExerciseDisplayOrder.Handler(
				editor.GetHttpContext(),
				workout.Id.Value,
				exerciseIndex,
				new EditDisplayOrderRequest(displayOrder),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
		if (result.Result is not Ok or NoContent) return;

		await GetWorkoutExercise.Handler(
				editor.GetHttpContext(),
				workout.Id.Value,
				exerciseIndex,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(typeof(Ok));

		var response = (Ok<GetWorkoutExerciseSetResponse>)result.Result;
		await Assert.That(response.Value!.DisplayOrder).IsEqualTo(displayOrder);
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

		const int index = 0;
		workout.Exercises.Add(new Workout.Exercise(workout.Id, index, exerciseInfo.Id, 0));
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
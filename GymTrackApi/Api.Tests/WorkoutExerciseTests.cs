using Api.Dtos;
using Api.Routes.App.Workouts.Exercises;
using Api.Tests.Mocks;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Tests;

internal sealed class WorkoutExerciseTests
{
	public static IEnumerable<(IUserInfo creator, IUserInfo exerciseInfoOwner, int exerciseIndex, Type responseType)> CreateWorkoutExerciseData() =>
	[
		new(Users.Admin1, Users.Admin1, 0, typeof(Created)),
		new(Users.Admin1, Users.User1, -1, typeof(Created)),
		new(Users.User1, Users.Admin1, 0, typeof(Created)),
		new(Users.User2, Users.User1, 0, typeof(ForbidHttpResult))
	];

	public static IEnumerable<(IUserInfo owner, IUserInfo accessor, int exerciseIndex, int accessedExerciseIndex, Type responseType)> GetWorkoutExerciseData() =>
	[
		new(Users.Admin1, Users.User1, 0, 0, typeof(Ok<GetWorkoutExerciseResponse>)),
		new(Users.Admin1, Users.User1, 0, 1, typeof(NotFound<string>)),
		new(Users.User1, Users.User1, 0, 0, typeof(Ok<GetWorkoutExerciseResponse>)),
		new(Users.User2, Users.User1, 0, 0, typeof(ForbidHttpResult)),
		new(Users.User1, Users.Admin1, 0, -1, typeof(NotFound<string>)),
		new(Users.User1, Users.Admin1, 0, 0, typeof(Ok<GetWorkoutExerciseResponse>))
	];

	public static IEnumerable<(IUserInfo owner, IUserInfo editor, string workoutName, Type responseType, Action<Workout>? configureWorkout)> EditWorkoutExerciseData() =>
	[
		new(Users.Admin1, Users.User1, "ValidName", typeof(ForbidHttpResult), null),
		new(Users.Admin1, Users.Admin1, "ValidName", typeof(NoContent), null),
		new(Users.Admin1, Users.Admin1, "ValidName", typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User2.Id, workout.Id))),
		new(Users.User1, Users.User1, "ValidName", typeof(NoContent), null),
		new(Users.User1, Users.User1, "ValidName", typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User2.Id, workout.Id))),
		new(Users.User2, Users.User1, "ValidName", typeof(ForbidHttpResult), null),
		new(Users.User2, Users.User1, "ValidName", typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User1.Id, workout.Id)))
	];

	public static IEnumerable<(IUserInfo owner, IUserInfo deleter, Type responseType, Action<Workout>? configureWorkout)> DeleteWorkoutExerciseData() =>
	[
		new(Users.Admin1, Users.User1, typeof(ForbidHttpResult), null),
		new(Users.Admin1, Users.Admin1, typeof(NoContent), null),
		new(Users.Admin1, Users.Admin1, typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User2.Id, workout.Id))),
		new(Users.User1, Users.User1, typeof(NoContent), null),
		new(Users.User1, Users.User1, typeof(ForbidHttpResult), workout => workout.Users.Add(new UserWorkout(Users.User2.Id, workout.Id))),
		new(Users.User2, Users.User1, typeof(ForbidHttpResult), null)
	];

	[Test]
	[MethodDataSource(nameof(CreateWorkoutExerciseData))]
	public async Task CreateWorkoutExercise_ReturnsCorrectResponse(IUserInfo creator, IUserInfo exerciseInfoOwner, int exerciseIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.Admin1)
			.WithUser(Users.User1)
			.WithUser(Users.User2)
			.WithWorkout(out var workout, creator.GetHttpContext().User)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, exerciseInfoOwner.GetHttpContext().User)
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

	[Test]
	[MethodDataSource(nameof(GetWorkoutExerciseData))]
	public async Task GetWorkoutExercise_ReturnsCorrectResponse(IUserInfo workoutOwner, IUserInfo accessor, int exerciseIndex, int accessedExerciseIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithUser(Users.User1)
			.WithUser(Users.User2)
			.WithUser(Users.Admin1)
			.WithWorkout(out var workout, workoutOwner.GetHttpContext().User)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, workoutOwner.GetHttpContext().User)
			.Build()
			.ConfigureAwait(false);

		workout.Exercises.Add(new Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id));
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

	// [Test]
	// [MethodDataSource(nameof(EditWorkoutExerciseData))]
	// public async Task EditWorkoutExercise_ReturnsCorrectResponse(IUserInfo workoutOwner, IUserInfo editor, string workoutName, Type responseType)
	// {
	// 	using var dataContext = await MockDataContextBuilder.CreateEmpty()
	// 		.WithUser(workoutOwner)
	// 		.WithUser(editor)
	// 		.WithWorkout(out var workout, workoutOwner.GetHttpContext().User)
	// 		.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, workoutOwner.GetHttpContext().User)
	// 		.Build()
	// 		.ConfigureAwait(false);
	//
	// 	workout.Exercises.Add(new Workout.Exercise(workout.Id, 0, exerciseInfo.Id));
	// 	await dataContext.SaveChangesAsync(default).ConfigureAwait(false);
	//
	// 	var result = await EditWorkout.Handler(
	// 			editor.GetHttpContext(),
	// 			workout.Id.Value,
	// 			new EditWorkoutExercise(workoutName),
	// 			dataContext,
	// 			CancellationToken.None)
	// 		.ConfigureAwait(false);
	//
	// 	await Assert.That(result.Result).IsTypeOf(responseType);
	// }

	// [Test]
	// [MethodDataSource(nameof(DeleteWorkoutExerciseData))]
	// public async Task DeleteWorkoutExercise_ReturnsCorrectResponse(IUserInfo? workoutOwner, IUserInfo deleter, Type responseType)
	// {
	// 	using var dataContext = await MockDataContextBuilder.CreateEmpty()
	// 		.WithUser(workoutOwner ?? Users.Admin1)
	// 		.WithUser(deleter)
	// 		.WithWorkout(out var workout, workoutOwner?.GetHttpContext().User)
	// 		.Build()
	// 		.ConfigureAwait(false);
	//
	// 	var result = await DeleteWorkout.Handler(
	// 			deleter.GetHttpContext(),
	// 			workout.Id.Value,
	// 			dataContext,
	// 			CancellationToken.None)
	// 		.ConfigureAwait(false);
	//
	// 	await Assert.That(result.Result).IsTypeOf(responseType);
	// }
}
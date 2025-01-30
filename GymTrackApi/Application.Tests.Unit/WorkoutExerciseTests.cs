using Application.Tests.Unit.Mocks;
using Application.Workout.Exercise.Commands;
using Application.Workout.Exercise.DisplayOrder.Commands;
using Application.Workout.Exercise.Dtos;
using Application.Workout.Exercise.Queries;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;
using Infrastructure.Persistence;
using OneOf.Types;

namespace Application.Tests.Unit;

internal sealed class WorkoutExerciseTests
{
	public static IEnumerable<(IUserInfo creator, IUserInfo exerciseInfoOwner, Type responseType)>
		CreateWorkoutExerciseData() =>
	[
		(Users.Admin1, Users.Admin1, typeof(Success<GetWorkoutExerciseResponse>)),
		(Users.Admin1, Users.User1, typeof(NotFound)),
		(Users.User1, Users.Admin1, typeof(NotFound)),
		(Users.User2, Users.User1, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(CreateWorkoutExerciseData))]
	public async Task CreateWorkoutExercise_ReturnsCorrectResponse(
		IUserInfo creator,
		IUserInfo exerciseInfoOwner,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, creator)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, exerciseInfoOwner)
			.Build()
			.ConfigureAwait(false);

		var handler = new CreateWorkoutExerciseHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new CreateWorkoutExerciseCommand(
				workout.Id,
				exerciseInfo.Id,
				creator.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo workoutOwner, IUserInfo accessor, int accessedExerciseIndex, Type responseType)>
		GetWorkoutExerciseData() =>
	[
		(Users.Admin1, Users.User1, 0, typeof(NotFound)),
		(Users.Admin1, Users.Admin1, 0, typeof(Success<GetWorkoutExerciseResponse>)),
		(Users.User1, Users.User1, 0, typeof(Success<GetWorkoutExerciseResponse>)),
		(Users.User1, Users.User1, 1, typeof(NotFound)),
		(Users.User2, Users.User1, 0, typeof(NotFound)),
		(Users.User1, Users.Admin1, -1, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(GetWorkoutExerciseData))]
	public async Task GetWorkoutExercise_ReturnsCorrectResponse(
		IUserInfo workoutOwner,
		IUserInfo accessor,
		int accessedExerciseIndex,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwner)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, workoutOwner)
			.Build()
			.ConfigureAwait(false);

		var exerciseIndex = WorkoutExerciseIndex.From(0);
		workout.Exercises.Add(new Domain.Models.Workout.Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0));
		await dataContext.SaveChangesAsync();

		var handler = new GetWorkoutExerciseHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new GetWorkoutExerciseQuery(workout.Id, accessedExerciseIndex, accessor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo owner, IUserInfo editor, int displayOrder, Type responseType)>
		UpdateWorkoutExerciseDisplayOrderData() =>
	[
		(Users.Admin1, Users.Admin1, 4, typeof(Success)),
		(Users.Admin1, Users.User1, 6, typeof(NotFound)),
		(Users.User1, Users.User1, 2, typeof(Success)),
		(Users.User1, Users.User1, 7, typeof(Success))
	];

	[Test]
	[MethodDataSource(nameof(UpdateWorkoutExerciseDisplayOrderData))]
	public async Task UpdateWorkoutExerciseDisplayOrder_ReturnsCorrectResponse(
		IUserInfo owner,
		IUserInfo editor,
		int displayOrder,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owner)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.All, owner)
			.Build()
			.ConfigureAwait(false);

		var exerciseIndex = WorkoutExerciseIndex.From(0);
		var exercise = new Domain.Models.Workout.Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.Exercises.Add(exercise);
		await dataContext.SaveChangesAsync();

		var handler = new UpdateWorkoutExerciseDisplayOrderHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new UpdateWorkoutExerciseDisplayOrderCommand(
				workout.Id,
				exerciseIndex,
				displayOrder,
				editor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo workoutOwner, IUserInfo deleter, int deletedExerciseIndex, Type responseType)>
		DeleteWorkoutExerciseData() =>
	[
		(Users.Admin1, Users.User1, 0, typeof(NotFound)),
		(Users.Admin1, Users.Admin1, 0, typeof(Success)),
		(Users.User1, Users.User1, 0, typeof(Success)),
		(Users.User2, Users.User1, 0, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(DeleteWorkoutExerciseData))]
	public async Task DeleteWorkoutExercise_ReturnsCorrectResponse(
		IUserInfo workoutOwner,
		IUserInfo deleter,
		int deletedExerciseIndex,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwner)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Distance, workoutOwner)
			.Build()
			.ConfigureAwait(false);

		var exerciseIndex = WorkoutExerciseIndex.From(0);
		workout.Exercises.Add(new Domain.Models.Workout.Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0));
		await dataContext.SaveChangesAsync();

		var handler = new DeleteWorkoutExerciseHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new DeleteWorkoutExerciseCommand(
				workout.Id,
				deletedExerciseIndex,
				deleter.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}
}
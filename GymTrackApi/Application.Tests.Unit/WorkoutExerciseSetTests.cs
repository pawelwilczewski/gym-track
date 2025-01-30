using Application.Tests.Unit.Mocks;
using Application.Workout.Exercise.Set.Commands;
using Application.Workout.Exercise.Set.DisplayOrder.Commands;
using Application.Workout.Exercise.Set.Dtos;
using Application.Workout.Exercise.Set.Queries;
using Domain.Common.Results;
using Domain.Models;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;
using Infrastructure.Persistence;
using OneOf.Types;

namespace Application.Tests.Unit;

internal sealed class WorkoutExerciseSetTests
{
	public static IEnumerable<(IUserInfo workoutOwner, IUserInfo creator, ExerciseMetricType metricType,
		ExerciseMetric metric, Reps reps, Type responseType)> CreateWorkoutExerciseSetData() =>
	[
		(Users.User2, Users.User1, ExerciseMetricType.Distance,
			new Distance(Amount.From(120.0), Distance.Unit.Metre), Reps.From(3), typeof(NotFound)),
		(Users.Admin1, Users.Admin1, ExerciseMetricType.Distance,
			new Distance(Amount.From(120.0), Distance.Unit.Metre), Reps.From(3), typeof(Success<GetWorkoutExerciseSetResponse>)),
		(Users.User1, Users.User1, ExerciseMetricType.Weight,
			new Distance(Amount.From(120.0), Distance.Unit.Metre), Reps.From(1), typeof(ValidationError)),
		(Users.User1, Users.User1, ExerciseMetricType.Weight,
			new Weight(Amount.From(120.0), Weight.Unit.Kilogram), Reps.From(1), typeof(Success<GetWorkoutExerciseSetResponse>)),
		(Users.User1, Users.User1, ExerciseMetricType.Duration,
			new Duration(TimeSpan.FromSeconds(1000.0)), Reps.From(1), typeof(Success<GetWorkoutExerciseSetResponse>))
	];

	[Test]
	[MethodDataSource(nameof(CreateWorkoutExerciseSetData))]
	public async Task CreateWorkoutExerciseSet_ReturnsCorrectResponse(
		IUserInfo workoutOwner,
		IUserInfo creator,
		ExerciseMetricType metricType,
		ExerciseMetric metric,
		Reps reps,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwner)
			.WithExerciseInfo(out var exerciseInfo, metricType, workoutOwner)
			.Build();

		var exerciseIndex = WorkoutExerciseIndex.From(0);
		workout.AddExercise(
			new Domain.Models.Workout.Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0),
			workoutOwner.Id);
		await dataContext.SaveChangesAsync();

		var handler = new CreateWorkoutExerciseSetHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new CreateWorkoutExerciseSetCommand(
				workout.Id,
				exerciseIndex,
				metric,
				reps,
				creator.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo workoutOwner, IUserInfo accessor,
		int accessedSetIndex, Type responseType)> GetWorkoutExerciseSetData() =>
	[
		(Users.Admin1, Users.User1, 0, typeof(NotFound)),
		(Users.Admin1, Users.Admin1, 0, typeof(Success<GetWorkoutExerciseSetResponse>)),
		(Users.User1, Users.User1, 1, typeof(NotFound)),
		(Users.User2, Users.User1, 0, typeof(NotFound)),
		(Users.User1, Users.Admin1, -1, typeof(NotFound)),
		(Users.User1, Users.Admin1, 0, typeof(NotFound))
	];

	[Test]
	[MethodDataSource(nameof(GetWorkoutExerciseSetData))]
	public async Task GetWorkoutExerciseSet_ReturnsCorrectResponse(
		IUserInfo workoutOwner,
		IUserInfo accessor,
		int accessedSetIndex,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwner)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Duration, workoutOwner)
			.Build();

		var exerciseIndex = WorkoutExerciseIndex.From(0);
		var setIndex = WorkoutExerciseSetIndex.From(0);
		var exercise = new Domain.Models.Workout.Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.AddExercise(exercise, workoutOwner.Id);
		await dataContext.SaveChangesAsync();

		if (!Domain.Models.Workout.Workout.Exercise.Set.TryCreate(
			exercise,
			setIndex,
			new Duration(TimeSpan.FromSeconds(1000.0)),
			Reps.From(2),
			0,
			workoutOwner.Id,
			out var set, out _))
		{
			throw new Exception("Invalid test case");
		}

		exercise.AddSet(set, workoutOwner.Id);
		await dataContext.SaveChangesAsync();

		var handler = new GetWorkoutExerciseSetHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new GetWorkoutExerciseSetQuery(
				workout.Id,
				exerciseIndex,
				accessedSetIndex,
				accessor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo workoutOwner, IUserInfo editor,
			ExerciseMetricType allowedMetricTypes, ExerciseMetric metric, int reps, Type responseType)>
		UpdateWorkoutExerciseSetData() =>
	[
		(Users.Admin1, Users.Admin1, ExerciseMetricType.Duration,
			new Duration(TimeSpan.FromSeconds(1000.0)), 4, typeof(Success)),
		(Users.User1, Users.User1, ExerciseMetricType.Duration | ExerciseMetricType.Weight,
			new Weight(Amount.From(120.0), Weight.Unit.Pound), 4, typeof(Success))
	];

	[Test]
	[MethodDataSource(nameof(UpdateWorkoutExerciseSetData))]
	public async Task UpdateWorkoutExerciseSet_ReturnsCorrectResponse(
		IUserInfo workoutOwner,
		IUserInfo editor,
		ExerciseMetricType allowedMetricTypes,
		ExerciseMetric metric,
		int reps,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwner)
			.WithExerciseInfo(out var exerciseInfo, allowedMetricTypes, workoutOwner)
			.Build();

		var exerciseIndex = WorkoutExerciseIndex.From(0);
		var setIndex = WorkoutExerciseSetIndex.From(0);
		var exercise = new Domain.Models.Workout.Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.AddExercise(exercise, workoutOwner.Id);
		await dataContext.SaveChangesAsync();

		if (!Domain.Models.Workout.Workout.Exercise.Set.TryCreate(
			exercise,
			setIndex,
			new Duration(TimeSpan.FromSeconds(1000.0)),
			Reps.From(2),
			0,
			workoutOwner.Id, out var set, out _))
		{
			throw new Exception("Invalid test case");
		}

		exercise.AddSet(set, workoutOwner.Id);

		await dataContext.SaveChangesAsync();

		var handler = new UpdateWorkoutExerciseSetHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new UpdateWorkoutExerciseSetCommand(
				workout.Id,
				exerciseIndex,
				setIndex,
				metric,
				Reps.From(reps),
				editor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo workoutOwner, IUserInfo editor,
		int displayOrder, Type responseType)> UpdateWorkoutExerciseSetDisplayOrderData() =>
	[
		(Users.Admin1, Users.Admin1, 4, typeof(Success)),
		(Users.User1, Users.User1, 2, typeof(Success)),
		(Users.User1, Users.User1, 7, typeof(Success))
	];

	[Test]
	[MethodDataSource(nameof(UpdateWorkoutExerciseSetDisplayOrderData))]
	public async Task UpdateWorkoutExerciseSetDisplayOrder_ReturnsCorrectResponse(
		IUserInfo workoutOwner,
		IUserInfo editor,
		int displayOrder,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwner)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.All, workoutOwner)
			.Build();

		var exerciseIndex = WorkoutExerciseIndex.From(0);
		var setIndex = WorkoutExerciseSetIndex.From(0);
		var exercise = new Domain.Models.Workout.Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.AddExercise(exercise, workoutOwner.Id);
		await dataContext.SaveChangesAsync();

		if (!Domain.Models.Workout.Workout.Exercise.Set.TryCreate(
			exercise,
			setIndex,
			new Duration(TimeSpan.FromSeconds(1000.0)),
			Reps.From(2),
			0,
			workoutOwner.Id, out var set, out _))
		{
			throw new Exception("Invalid test case");
		}

		exercise.AddSet(set, workoutOwner.Id);

		await dataContext.SaveChangesAsync();

		var handler = new UpdateWorkoutExerciseSetDisplayOrderHandler(
			new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new UpdateWorkoutExerciseSetDisplayOrderCommand(
				workout.Id,
				exerciseIndex,
				setIndex,
				displayOrder,
				editor.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}

	public static IEnumerable<(IUserInfo workoutOwner, IUserInfo deleter,
		int deletedSetIndex, Type responseType)> DeleteWorkoutExerciseSetData() =>
	[
		(Users.Admin1, Users.User1, 0, typeof(NotFound)),
		(Users.Admin1, Users.Admin1, 0, typeof(Success)),
		(Users.User1, Users.User1, 0, typeof(Success))
	];

	[Test]
	[MethodDataSource(nameof(DeleteWorkoutExerciseSetData))]
	public async Task DeleteWorkoutExerciseSet_ReturnsCorrectResponse(
		IUserInfo workoutOwner,
		IUserInfo deleter,
		int deletedSetIndex,
		Type responseType)
	{
		await using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwner)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Duration, workoutOwner)
			.Build();

		var exerciseIndex = WorkoutExerciseIndex.From(0);
		var setIndex = WorkoutExerciseSetIndex.From(0);
		var exercise = new Domain.Models.Workout.Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.AddExercise(exercise, workoutOwner.Id);
		await dataContext.SaveChangesAsync();

		if (!Domain.Models.Workout.Workout.Exercise.Set.TryCreate(
			exercise,
			setIndex,
			new Duration(TimeSpan.FromSeconds(1000.0)),
			Reps.From(2),
			0,
			workoutOwner.Id, out var set, out _))
		{
			throw new Exception("Invalid test case");
		}

		exercise.AddSet(set, workoutOwner.Id);

		await dataContext.SaveChangesAsync();

		var handler = new DeleteWorkoutExerciseSetHandler(new UserDataContextFactory(dataContext));
		var result = await handler.Handle(
			new DeleteWorkoutExerciseSetCommand(
				workout.Id,
				exerciseIndex,
				deletedSetIndex,
				deleter.Id),
			CancellationToken.None);

		await Assert.That(result.Value).IsTypeOf(responseType);
	}
}
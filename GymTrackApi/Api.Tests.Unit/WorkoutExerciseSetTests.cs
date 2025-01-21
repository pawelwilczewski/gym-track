using Api.Dtos;
using Api.Routes.App.Workouts.Exercises.Sets;
using Api.Routes.App.Workouts.Exercises.Sets.DisplayOrder;
using Api.Tests.Unit.Mocks;
using Domain.Models;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Api.Tests.Unit;

internal sealed class WorkoutExerciseSetTests
{
	public static IEnumerable<(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo creator, ExerciseMetricType metricType, ExerciseMetric metric, int reps, Type responseType)> CreateWorkoutExerciseSetData()
	{
		Amount.TryCreate(120.0, out var amount);

		return
		[
			new([Users.User2], Users.User1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 3, typeof(ForbidHttpResult)),
			new([Users.Admin1], Users.Admin1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 3, typeof(Created)),
			new([Users.Admin1], Users.Admin1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 3, typeof(Created)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 3, typeof(Created)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), -1, typeof(ValidationProblem)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 0, typeof(ValidationProblem)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Weight(amount, Weight.Unit.Kilogram), 0, typeof(ValidationProblem)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Weight(amount, Weight.Unit.Kilogram), 1, typeof(ValidationProblem)),
			new([Users.User1], Users.User1, ExerciseMetricType.Weight, new Weight(amount, Weight.Unit.Kilogram), 1, typeof(Created)),
			new([Users.User1], Users.User1, ExerciseMetricType.Duration, new Duration(TimeSpan.FromSeconds(1000.0)), 1, typeof(Created))
		];
	}

	[Test]
	[MethodDataSource(nameof(CreateWorkoutExerciseSetData))]
	public async Task CreateWorkoutExerciseSet_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo creator, ExerciseMetricType metricType, ExerciseMetric metric, int reps, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwners)
			.WithExerciseInfo(out var exerciseInfo, metricType, workoutOwners)
			.Build()
			.ConfigureAwait(false);

		const int index = 0;
		workout.Exercises.Add(new Workout.Exercise(workout.Id, index, exerciseInfo.Id, 0));
		await dataContext.SaveChangesAsync(default).ConfigureAwait(false);

		var result = await CreateWorkoutExerciseSet.Handler(
				creator.GetHttpContext(),
				workout.Id.Value,
				index,
				new CreateWorkoutExerciseSetRequest(metric, reps),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo accessor, int setIndex, int accessedSetIndex, Type responseType)> GetWorkoutExerciseSetData() =>
	[
		new([Users.Admin1], Users.User1, 0, 0, typeof(Ok<GetWorkoutExerciseSetResponse>)),
		new([Users.Admin1], Users.User1, 0, 1, typeof(NotFound<string>)),
		new([Users.User1], Users.User1, 0, 0, typeof(Ok<GetWorkoutExerciseSetResponse>)),
		new([Users.User1], Users.User1, 1, 1, typeof(Ok<GetWorkoutExerciseSetResponse>)),
		new([Users.User2], Users.User1, 0, 0, typeof(ForbidHttpResult)),
		new([Users.User1], Users.Admin1, 0, -1, typeof(NotFound<string>)),
		new([Users.User1], Users.Admin1, 0, 0, typeof(Ok<GetWorkoutExerciseSetResponse>))
	];

	[Test]
	[MethodDataSource(nameof(GetWorkoutExerciseSetData))]
	public async Task GetWorkoutExerciseSet_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo accessor, int addedSetIndex, int accessedSetIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwners)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Duration, workoutOwners)
			.Build()
			.ConfigureAwait(false);

		const int exerciseIndex = 0;
		if (!PositiveCount.TryCreate(2, out var reps)) throw new Exception("Invalid test case");

		var exercise = new Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.Exercises.Add(exercise);
		exercise.Sets.Add(new Workout.Exercise.Set(exercise, addedSetIndex, new Duration(TimeSpan.FromSeconds(1000.0)), reps, 0));
		await dataContext.SaveChangesAsync(default).ConfigureAwait(false);

		var result = await GetWorkoutExerciseSet.Handler(
				accessor.GetHttpContext(),
				workout.Id.Value,
				exerciseIndex,
				accessedSetIndex,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo editor, ExerciseMetricType allowedMetricTypes, ExerciseMetric metric, int reps, Type responseType)> EditWorkoutExerciseSetData()
	{
		Amount.TryCreate(120.0, out var amount);

		return
		[
			new([], Users.User1, ExerciseMetricType.Duration, new Duration(TimeSpan.FromSeconds(1000.0)), 4, typeof(ForbidHttpResult)),
			new([Users.Admin1], Users.Admin1, ExerciseMetricType.Duration, new Duration(TimeSpan.FromSeconds(1000.0)), 4, typeof(NoContent)),
			new([Users.Admin1, Users.User2], Users.Admin1, ExerciseMetricType.Duration, new Duration(TimeSpan.FromSeconds(1000.0)), 4, typeof(NoContent)),
			new([Users.User1], Users.User1, ExerciseMetricType.Duration, new Duration(TimeSpan.FromSeconds(1000.0)), 4, typeof(NoContent)),
			new([Users.User1, Users.User2], Users.User1, ExerciseMetricType.Duration, new Duration(TimeSpan.FromSeconds(1000.0)), 4, typeof(ForbidHttpResult)),
			new([Users.User2], Users.User1, ExerciseMetricType.Duration, new Duration(TimeSpan.FromSeconds(1000.0)), 4, typeof(ForbidHttpResult)),
			new([Users.User1], Users.User1, ExerciseMetricType.Duration | ExerciseMetricType.Weight, new Weight(amount, Weight.Unit.Pound), 4, typeof(NoContent)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Weight(amount, Weight.Unit.Pound), 4, typeof(ValidationProblem))
		];
	}

	[Test]
	[MethodDataSource(nameof(EditWorkoutExerciseSetData))]
	public async Task EditWorkoutExerciseSet_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo editor, ExerciseMetricType allowedMetricTypes, ExerciseMetric metric, int reps, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owners)
			.WithExerciseInfo(out var exerciseInfo, allowedMetricTypes, owners)
			.Build()
			.ConfigureAwait(false);

		const int exerciseIndex = 0;
		const int setIndex = 0;
		if (!PositiveCount.TryCreate(2, out var originalReps)) throw new Exception("Invalid test case");

		var exercise = new Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.Exercises.Add(exercise);
		exercise.Sets.Add(new Workout.Exercise.Set(exercise, setIndex, new Duration(TimeSpan.FromSeconds(1000.0)), originalReps, 0));
		await dataContext.SaveChangesAsync(default).ConfigureAwait(false);

		var result = await EditWorkoutExerciseSet.Handler(
				editor.GetHttpContext(),
				workout.Id.Value,
				exerciseIndex,
				setIndex,
				new EditWorkoutExerciseSetRequest(metric, reps),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> owners, IUserInfo editor, int displayOrder, Type responseType)> EditWorkoutExerciseSetDisplayOrderData() =>
	[
		new([Users.Admin1], Users.Admin1, 4, typeof(NoContent)),
		new([Users.Admin1], Users.User1, 6, typeof(ForbidHttpResult)),
		new([Users.User1], Users.User1, 2, typeof(NoContent)),
		new([Users.User1], Users.User1, 7, typeof(NoContent))
	];

	[Test]
	[MethodDataSource(nameof(EditWorkoutExerciseSetDisplayOrderData))]
	public async Task EditWorkoutExerciseSetDisplayOrder_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> owners, IUserInfo editor, int displayOrder, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, owners)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.All, owners)
			.Build()
			.ConfigureAwait(false);

		const int exerciseIndex = 0;
		const int setIndex = 0;
		if (!PositiveCount.TryCreate(2, out var originalReps)) throw new Exception("Invalid test case");

		var exercise = new Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.Exercises.Add(exercise);
		exercise.Sets.Add(new Workout.Exercise.Set(exercise, setIndex, new Duration(TimeSpan.FromSeconds(1000.0)), originalReps, 0));
		await dataContext.SaveChangesAsync(default).ConfigureAwait(false);

		var result = await EditWorkoutExerciseSetDisplayOrder.Handler(
				editor.GetHttpContext(),
				workout.Id.Value,
				exerciseIndex,
				setIndex,
				new EditDisplayOrderRequest(displayOrder),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
		if (result.Result is not Ok or NoContent) return;

		await GetWorkoutExerciseSet.Handler(
				editor.GetHttpContext(),
				workout.Id.Value,
				exerciseIndex,
				setIndex,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(typeof(Ok));

		var response = (Ok<GetWorkoutExerciseSetResponse>)result.Result;
		await Assert.That(response.Value!.DisplayOrder).IsEqualTo(displayOrder);
	}

	public static IEnumerable<(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo deleter, int exerciseIndex, int deletedExerciseIndex, Type responseType)> DeleteWorkoutExerciseSetData() =>
	[
		new([Users.Admin1], Users.User1, 0, 0, typeof(ForbidHttpResult)),
		new([Users.Admin1], Users.Admin1, 0, 0, typeof(NoContent)),
		new([Users.Admin1, Users.User2], Users.Admin1, 0, 0, typeof(NoContent)),
		new([Users.User1], Users.User1, 0, 0, typeof(NoContent)),
		new([Users.User1, Users.User2], Users.User1, 0, 0, typeof(ForbidHttpResult)),
		new([Users.User2], Users.User1, 0, 0, typeof(ForbidHttpResult))
	];

	[Test]
	[MethodDataSource(nameof(DeleteWorkoutExerciseSetData))]
	public async Task DeleteWorkoutExerciseSet_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo deleter, int setIndex, int deletedSetIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwners)
			.WithExerciseInfo(out var exerciseInfo, ExerciseMetricType.Duration, workoutOwners)
			.Build()
			.ConfigureAwait(false);

		const int exerciseIndex = 0;
		const int index = 0;
		if (!PositiveCount.TryCreate(2, out var reps)) throw new Exception("Invalid test case");

		var exercise = new Workout.Exercise(workout.Id, exerciseIndex, exerciseInfo.Id, 0);
		workout.Exercises.Add(exercise);
		exercise.Sets.Add(new Workout.Exercise.Set(exercise, index, new Duration(TimeSpan.FromSeconds(1000.0)), reps, 0));
		await dataContext.SaveChangesAsync(default).ConfigureAwait(false);

		var result = await DeleteWorkoutExerciseSet.Handler(
				deleter.GetHttpContext(),
				workout.Id.Value,
				exerciseIndex,
				deletedSetIndex,
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}
}
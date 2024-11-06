using Api.Dtos;
using Api.Routes.App.Workouts.Exercises.Sets;
using Api.Tests.Mocks;
using Domain.Models;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Index = Domain.Models.Index;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Api.Tests;

internal sealed class WorkoutExerciseSetTests
{
	public static IEnumerable<(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo creator, ExerciseMetricType metricType, ExerciseMetric metric, int reps, int setIndex, Type responseType)> CreateWorkoutExerciseSetData()
	{
		Amount.TryCreate(120.0, out var amount);

		return
		[
			new([Users.User2], Users.User1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 3, 0, typeof(ForbidHttpResult)),
			new([Users.Admin1], Users.Admin1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 3, 0, typeof(Created)),
			new([Users.Admin1], Users.Admin1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 3, -1, typeof(ValidationProblem)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 3, 0, typeof(Created)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), -1, 0, typeof(ValidationProblem)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Distance(amount, Distance.Unit.Metre), 0, 0, typeof(ValidationProblem)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Weight(amount, Weight.Unit.Kilogram), 0, 0, typeof(ValidationProblem)),
			new([Users.User1], Users.User1, ExerciseMetricType.Distance, new Weight(amount, Weight.Unit.Kilogram), 1, 0, typeof(ValidationProblem)),
			new([Users.User1], Users.User1, ExerciseMetricType.Weight, new Weight(amount, Weight.Unit.Kilogram), 1, 0, typeof(Created)),
			new([Users.User1], Users.User1, ExerciseMetricType.Duration, new Duration(TimeSpan.FromSeconds(1000.0)), 1, 0, typeof(Created))
		];
	}

	[Test]
	[MethodDataSource(nameof(CreateWorkoutExerciseSetData))]
	public async Task CreateWorkoutExerciseSet_ReturnsCorrectResponse(IReadOnlyList<IUserInfo> workoutOwners, IUserInfo creator, ExerciseMetricType metricType, ExerciseMetric metric, int reps, int setIndex, Type responseType)
	{
		using var dataContext = await MockDataContextBuilder.CreateEmpty()
			.WithAllUsers()
			.WithWorkout(out var workout, workoutOwners)
			.WithExerciseInfo(out var exerciseInfo, metricType, workoutOwners)
			.Build()
			.ConfigureAwait(false);

		if (!Index.TryCreate(0, out var index)) throw new Exception("Invalid test case");

		workout.Exercises.Add(new Workout.Exercise(workout.Id, index, exerciseInfo.Id));
		await dataContext.SaveChangesAsync(default).ConfigureAwait(false);

		var result = await CreateWorkoutExerciseSet.Handler(
				creator.GetHttpContext(),
				workout.Id.Value,
				index.IntValue,
				new CreateWorkoutExerciseSetRequest(setIndex, metric, reps),
				dataContext,
				CancellationToken.None)
			.ConfigureAwait(false);

		await Assert.That(result.Result).IsTypeOf(responseType);
	}
}
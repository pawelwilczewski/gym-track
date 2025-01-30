using Api.Routes.App.Workouts.Exercises;
using Api.Routes.App.Workouts.Exercises.DisplayOrder;
using Api.Routes.App.Workouts.Exercises.Sets;
using Api.Routes.App.Workouts.Exercises.Sets.DisplayOrder;

namespace Api.Routes.App.Workouts;

internal static class WorkoutRoutes
{
	public static IEndpointRouteBuilder MapWorkoutRoutes(this IEndpointRouteBuilder builder)
	{
		var workouts = builder.MapGroup("workouts")
			.RequireAuthorization()
			.WithTags("Workout")
			.Map(new CreateWorkout())
			.Map(new GetWorkouts())
			.Map(new GetWorkout())
			.Map(new UpdateWorkout())
			.Map(new DeleteWorkout());

		var exercises = workouts.MapGroup("{workoutId:guid}/exercises")
			.Map(new CreateWorkoutExercise())
			.Map(new GetWorkoutExercise())
			.Map(new DeleteWorkoutExercise());

		var exercisesDisplayOrder = exercises.MapGroup("{exerciseIndex:int}/display-order")
			.Map(new UpdateWorkoutExerciseDisplayOrder());

		var sets = exercises.MapGroup("{exerciseIndex:int}/sets")
			.Map(new CreateWorkoutExerciseSet())
			.Map(new DeleteWorkoutExerciseSet())
			.Map(new GetWorkoutExerciseSet())
			.Map(new UpdateWorkoutExerciseSet());

		var setsDisplayOrder = sets.MapGroup("{setIndex:int}/display-order")
			.Map(new UpdateWorkoutExerciseSetDisplayOrder());

		return builder;
	}
}
using Api.Routes.App.Workouts.Exercises.Sets;

namespace Api.Routes.App.Workouts.Exercises;

internal static class WorkoutExerciseRoutes
{
	public static IEndpointRouteBuilder MapWorkoutExerciseRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("{workoutId:guid}/exercises")
			.RequireAuthorization()
			.Map(new CreateWorkoutExercise())
			.MapWorkoutExerciseSetRoutes();

		return builder;
	}
}
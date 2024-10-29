using Api.Routes.App.Workouts.Exercises;

namespace Api.Routes.App.Workouts;

internal static class WorkoutRoutes
{
	public static IEndpointRouteBuilder MapWorkoutRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("workouts")
			.RequireAuthorization()
			.WithTags("Workout")
			.Map(new CreateWorkout())
			.Map(new GetWorkout())
			.Map(new EditWorkout())
			.Map(new DeleteWorkout())
			.MapWorkoutExerciseRoutes();

		return builder;
	}
}
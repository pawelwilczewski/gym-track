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
			.Map(new GetWorkouts())
			.Map(new GetWorkout())
			.Map(new UpdateWorkout())
			.Map(new DeleteWorkout())
			.MapWorkoutExerciseRoutes();

		return builder;
	}
}
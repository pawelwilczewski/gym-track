namespace Api.Routes.Workout;

internal static class WorkoutRoutes
{
	public static IEndpointRouteBuilder MapWorkoutRoutes(this IEndpointRouteBuilder builder)
	{
		var root = builder.MapGroup("workout");
		root
			.Map(new CreateWorkout())
			.Map(new DeleteWorkout());

		return builder;
	}
}
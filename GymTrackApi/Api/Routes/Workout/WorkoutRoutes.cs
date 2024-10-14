namespace Api.Routes.Workout;

internal static class WorkoutRoutes
{
	public static IEndpointRouteBuilder MapWorkoutRoutes(this IEndpointRouteBuilder builder)
	{
		var root = builder.MapGroup("workout");
		root
			.Map(new Create())
			.Map(new Delete());

		return builder;
	}
}
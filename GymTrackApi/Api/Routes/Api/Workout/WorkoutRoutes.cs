namespace Api.Routes.Api.Workout;

internal static class WorkoutRoutes
{
	public static IEndpointRouteBuilder MapWorkoutRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("workout")
			.Map(new CreateWorkout())
			.Map(new DeleteWorkout())
			.Map(new EditWorkout());

		return builder;
	}
}
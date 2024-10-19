namespace Api.Routes.Api.Workout;

internal static class WorkoutRoutes
{
	public static IEndpointRouteBuilder MapWorkoutRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("workout")
			.Map(new CreateWorkout())
			.Map(new GetWorkout())
			.Map(new EditWorkout())
			.Map(new DeleteWorkout());

		return builder;
	}
}
namespace Api.Routes.App.Workouts;

internal static class WorkoutRoutes
{
	public static IEndpointRouteBuilder MapWorkoutRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("workouts")
			.RequireAuthorization()
			.Map(new CreateWorkout())
			.Map(new GetWorkout())
			.Map(new EditWorkout())
			.Map(new DeleteWorkout());

		return builder;
	}
}
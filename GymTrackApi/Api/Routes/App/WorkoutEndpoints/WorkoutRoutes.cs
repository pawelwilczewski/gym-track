namespace Api.Routes.App.WorkoutEndpoints;

internal static class WorkoutRoutes
{
	public static IEndpointRouteBuilder MapWorkoutRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("workout")
			.RequireAuthorization()
			.Map(new CreateWorkout())
			.Map(new GetWorkout())
			.Map(new EditWorkout())
			.Map(new DeleteWorkout());

		return builder;
	}
}
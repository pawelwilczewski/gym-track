namespace Api.Routes.App.Tracking.Workouts;

internal static class TrackedWorkoutRoutes
{
	public static IEndpointRouteBuilder MapTrackedWorkoutRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("workouts")
			.Map(new GetTrackedWorkout())
			.Map(new GetTrackedWorkouts())
			.Map(new CreateTrackedWorkout())
			.Map(new UpdateTrackedWorkout())
			.Map(new DeleteTrackedWorkout());

		return builder;
	}
}
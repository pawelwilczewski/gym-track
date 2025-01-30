using Api.Routes.App.Tracking.Workouts;

namespace Api.Routes.App.Tracking;

internal static class TrackingRoutes
{
	public static IEndpointRouteBuilder MapTrackingRoutes(this IEndpointRouteBuilder builder)
	{
		var tracking = builder.MapGroup("tracking")
			.RequireAuthorization()
			.WithTags("Tracking");

		tracking.MapGroup("workouts")
			.Map(new GetTrackedWorkout())
			.Map(new GetTrackedWorkouts())
			.Map(new CreateTrackedWorkout())
			.Map(new UpdateTrackedWorkout())
			.Map(new DeleteTrackedWorkout());

		return builder;
	}
}
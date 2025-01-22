using Api.Routes.App.Tracking.Workouts;

namespace Api.Routes.App.Tracking;

internal static class TrackingRoutes
{
	public static IEndpointRouteBuilder MapTrackingRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("tracking")
			.RequireAuthorization()
			.WithTags("Tracking")
			.MapTrackedWorkoutRoutes();

		return builder;
	}
}
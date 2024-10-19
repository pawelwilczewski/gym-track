using Api.Routes.Api.Workout;

namespace Api.Routes.Api;

internal static class ApiRoutes
{
	public static IEndpointRouteBuilder MapApiRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("api")
			.MapWorkoutRoutes();

		return builder;
	}
}
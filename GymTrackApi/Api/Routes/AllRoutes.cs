using Api.Routes.Api;
using Api.Routes.Auth;

namespace Api.Routes;

internal static class AllRoutes
{
	public static IEndpointRouteBuilder MapAllRoutes(this IEndpointRouteBuilder builder)
	{
		builder
			.MapAuthRoutes()
			.MapApiRoutes();

		return builder;
	}
}
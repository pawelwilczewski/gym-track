using Api.Routes.Api.ExerciseInfo;
using Api.Routes.Api.Workout;

namespace Api.Routes.Api;

internal static class ApiRoutes
{
	public static IEndpointRouteBuilder MapApiRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("api")
			.MapWorkoutRoutes()
			.MapExerciseInfoRoutes();

		return builder;
	}
}
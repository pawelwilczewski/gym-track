using Api.Routes.App.ExerciseInfoEndpoints;
using Api.Routes.App.WorkoutEndpoints;

namespace Api.Routes.App;

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
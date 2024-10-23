using Api.Routes.App.ExerciseInfos;
using Api.Routes.App.Workouts;

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
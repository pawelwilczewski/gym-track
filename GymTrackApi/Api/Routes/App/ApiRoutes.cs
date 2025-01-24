using Api.Routes.App.ExerciseInfos;
using Api.Routes.App.Tracking;
using Api.Routes.App.Workouts;
using Asp.Versioning;

namespace Api.Routes.App;

internal static class ApiRoutes
{
	public static IEndpointRouteBuilder MapApiRoutes(this IEndpointRouteBuilder builder)
	{
		var apiVersionSet = builder
			.NewApiVersionSet()
			.HasApiVersion(new ApiVersion(1))
			.Build();

		builder.MapGroup("api/v{apiVersion:apiVersion}")
			.WithApiVersionSet(apiVersionSet)
			.MapWorkoutRoutes()
			.MapExerciseInfoRoutes()
			.MapTrackingRoutes();

		return builder;
	}
}
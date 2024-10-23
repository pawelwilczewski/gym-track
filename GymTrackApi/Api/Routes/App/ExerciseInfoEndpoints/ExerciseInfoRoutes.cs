using Api.Routes.App.ExerciseInfoEndpoints.Step;
using Api.Routes.App.ExerciseInfoEndpointsEndpoints;

namespace Api.Routes.App.ExerciseInfoEndpoints;

internal static class ExerciseInfoRoutes
{
	public static IEndpointRouteBuilder MapExerciseInfoRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("exerciseInfos")
			.Map(new CreateExerciseInfo())
			.Map(new GetExerciseInfo())
			.Map(new EditExerciseInfo())
			.Map(new DeleteExerciseInfo())
			.Map(new EditExerciseInfoThumbnail())
			.MapExerciseInfoStepRoutes();

		return builder;
	}
}
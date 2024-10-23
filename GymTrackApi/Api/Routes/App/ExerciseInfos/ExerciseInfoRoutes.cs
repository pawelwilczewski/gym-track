using Api.Routes.App.ExerciseInfoEndpointsEndpoints;
using Api.Routes.App.ExerciseInfos.Steps;

namespace Api.Routes.App.ExerciseInfos;

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
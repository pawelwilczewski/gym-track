using Api.Routes.Api.ExerciseInfo.Step;

namespace Api.Routes.Api.ExerciseInfo;

internal static class ExerciseInfoRoutes
{
	public static IEndpointRouteBuilder MapExerciseInfoRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("exerciseInfo")
			.Map(new CreateExerciseInfo())
			.Map(new GetExerciseInfo())
			.Map(new EditExerciseInfo())
			.Map(new DeleteExerciseInfo())
			.Map(new EditExerciseInfoThumbnail())
			.MapExerciseStepInfoRoutes();

		return builder;
	}
}
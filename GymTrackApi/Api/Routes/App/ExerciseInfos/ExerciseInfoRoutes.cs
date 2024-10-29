using Api.Routes.App.ExerciseInfos.Steps;

namespace Api.Routes.App.ExerciseInfos;

internal static class ExerciseInfoRoutes
{
	public static IEndpointRouteBuilder MapExerciseInfoRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("exerciseInfos")
			.RequireAuthorization()
			.WithTags("ExerciseInfo")
			.Map(new CreateExerciseInfo())
			.Map(new GetExerciseInfo())
			.Map(new EditExerciseInfo())
			.Map(new DeleteExerciseInfo())
			.Map(new EditExerciseInfoThumbnail())
			.MapExerciseInfoStepRoutes();

		return builder;
	}
}
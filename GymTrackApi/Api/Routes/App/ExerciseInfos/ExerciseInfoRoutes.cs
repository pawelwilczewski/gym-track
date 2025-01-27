using Api.Routes.App.ExerciseInfos.Steps;

namespace Api.Routes.App.ExerciseInfos;

internal static class ExerciseInfoRoutes
{
	public static IEndpointRouteBuilder MapExerciseInfoRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("exercise-infos")
			.RequireAuthorization()
			.WithTags("ExerciseInfo")
			.Map(new CreateExerciseInfo())
			.Map(new GetExerciseInfos())
			.Map(new GetExerciseInfo())
			.Map(new UpdateExerciseInfo())
			.Map(new DeleteExerciseInfo())
			.MapExerciseInfoStepRoutes();

		return builder;
	}
}
namespace Api.Routes.Api.ExerciseInfo;

internal static class ExerciseInfoRoutes
{
	public static IEndpointRouteBuilder MapExerciseInfoRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("exerciseInfo")
			.Map(new CreateExerciseInfo())
			.Map(new GetExerciseInfo())
			.Map(new EditExerciseInfo())
			.Map(new DeleteExerciseInfo());

		return builder;
	}
}
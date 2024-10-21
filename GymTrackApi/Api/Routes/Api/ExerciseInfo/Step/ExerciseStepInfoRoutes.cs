namespace Api.Routes.Api.ExerciseInfo.Step;

internal static class ExerciseStepInfoRoutes
{
	internal static IEndpointRouteBuilder MapExerciseStepInfoRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("stepInfo")
			.Map(new CreateExerciseStepInfo())
			.Map(new GetExerciseStepInfo())
			.Map(new EditExerciseStepInfo())
			.Map(new DeleteExerciseStepInfo());

		return builder;
	}
}
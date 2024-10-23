namespace Api.Routes.App.ExerciseInfoEndpoints.Step;

internal static class ExerciseInfoStepRoutes
{
	internal static IEndpointRouteBuilder MapExerciseInfoStepRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("/{exerciseInfoId:Guid}/step/")
			.Map(new CreateExerciseInfoStep())
			.Map(new GetExerciseInfoStep())
			.Map(new EditExerciseInfoStep())
			.Map(new DeleteExerciseInfoStep())
			.Map(new EditExerciseInfoStepImage());

		return builder;
	}
}
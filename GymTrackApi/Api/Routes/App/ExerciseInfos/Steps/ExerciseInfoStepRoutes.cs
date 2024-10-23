namespace Api.Routes.App.ExerciseInfos.Steps;

internal static class ExerciseInfoStepRoutes
{
	internal static IEndpointRouteBuilder MapExerciseInfoStepRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("/{exerciseInfoId:Guid}/steps/")
			.Map(new CreateExerciseInfoStep())
			.Map(new GetExerciseInfoStep())
			.Map(new EditExerciseInfoStep())
			.Map(new DeleteExerciseInfoStep())
			.Map(new EditExerciseInfoStepImage());

		return builder;
	}
}
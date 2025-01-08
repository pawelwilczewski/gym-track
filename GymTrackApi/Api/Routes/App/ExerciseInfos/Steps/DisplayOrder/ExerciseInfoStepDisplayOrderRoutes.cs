namespace Api.Routes.App.ExerciseInfos.Steps.DisplayOrder;

internal static class ExerciseInfoStepDisplayOrderRoutes
{
	internal static IEndpointRouteBuilder MapExerciseInfoStepDisplayOrderRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("{stepIndex:int}/display-order")
			.Map(new EditExerciseInfoStepDisplayOrder());

		return builder;
	}
}
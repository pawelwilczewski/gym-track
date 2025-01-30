using Api.Routes.App.ExerciseInfos.Steps;
using Api.Routes.App.ExerciseInfos.Steps.DisplayOrder;

namespace Api.Routes.App.ExerciseInfos;

internal static class ExerciseInfoRoutes
{
	public static IEndpointRouteBuilder MapExerciseInfoRoutes(this IEndpointRouteBuilder builder)
	{
		var exerciseInfos = builder.MapGroup("exercise-infos")
			.RequireAuthorization()
			.WithTags("ExerciseInfo")
			.Map(new CreateExerciseInfo())
			.Map(new GetExerciseInfos())
			.Map(new GetExerciseInfo())
			.Map(new UpdateExerciseInfo())
			.Map(new DeleteExerciseInfo());

		var steps = exerciseInfos.MapGroup("{exerciseInfoId:Guid}/steps/")
			.Map(new CreateExerciseInfoStep())
			.Map(new GetExerciseInfoStep())
			.Map(new UpdateExerciseInfoStep())
			.Map(new DeleteExerciseInfoStep());

		var stepsDisplayOrder = steps.MapGroup("{stepIndex:int}/display-order")
			.Map(new UpdateExerciseInfoStepDisplayOrder());

		return builder;
	}
}
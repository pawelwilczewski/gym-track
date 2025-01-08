namespace Api.Routes.App.Workouts.Exercises.DisplayOrder;

internal static class WorkoutExerciseDisplayOrderRoutes
{
	public static IEndpointRouteBuilder MapWorkoutExerciseDisplayOrderRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("{exerciseIndex:int}/display-order")
			.Map(new EditWorkoutExerciseDisplayOrder());

		return builder;
	}
}
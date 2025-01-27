namespace Api.Routes.App.Workouts.Exercises.Sets.DisplayOrder;

internal static class WorkoutExerciseSetDisplayOrderRoutes
{
	public static IEndpointRouteBuilder MapWorkoutExerciseSetDisplayOrderRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("{setIndex:int}/display-order")
			.Map(new UpdateWorkoutExerciseSetDisplayOrder());

		return builder;
	}
}
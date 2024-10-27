namespace Api.Routes.App.Workouts.Exercises.Sets;

internal static class WorkoutExerciseSetRoutes
{
	public static IEndpointRouteBuilder MapWorkoutExerciseSetRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("{exerciseIndex:int}/sets")
			.RequireAuthorization()
			.Map(new CreateWorkoutExerciseSet())
			.Map(new DeleteWorkoutExerciseSet())
			.Map(new GetWorkoutExerciseSet());

		return builder;
	}
}
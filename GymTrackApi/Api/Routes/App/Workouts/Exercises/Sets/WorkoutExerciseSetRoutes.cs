using Api.Routes.App.Workouts.Exercises.Sets.DisplayOrder;

namespace Api.Routes.App.Workouts.Exercises.Sets;

internal static class WorkoutExerciseSetRoutes
{
	public static IEndpointRouteBuilder MapWorkoutExerciseSetRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("{exerciseIndex:int}/sets")
			.Map(new CreateWorkoutExerciseSet())
			.Map(new DeleteWorkoutExerciseSet())
			.Map(new GetWorkoutExerciseSet())
			.Map(new UpdateWorkoutExerciseSet())
			.MapWorkoutExerciseSetDisplayOrderRoutes();

		return builder;
	}
}
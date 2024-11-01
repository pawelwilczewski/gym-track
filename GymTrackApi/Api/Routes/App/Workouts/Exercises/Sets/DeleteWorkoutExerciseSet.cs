using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts.Exercises.Sets;

internal sealed class DeleteWorkoutExerciseSet : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromRoute] int index,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var workoutIdTyped = new Id<Workout>(workoutId);
		var workout = await dataContext.Workouts.Include(workout => workout.Users)
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.Sets)
			.FirstOrDefaultAsync(workout => workout.Id == workoutIdTyped, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return TypedResults.NotFound("Workout not found.");
		if (!httpContext.User.CanModifyOrDelete(workout.Users)) return TypedResults.Forbid();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == index);
		if (exercise is null) return TypedResults.NotFound("Exercise not found.");

		var set = exercise.Sets.FirstOrDefault(set => set.Index == index);
		if (set is null) return TypedResults.NotFound("Set not found.");

		exercise.Sets.Remove(set);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{index:int}", Handler);
		return builder;
	}
}
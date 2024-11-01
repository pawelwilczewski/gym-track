using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts.Exercises;

internal sealed class EditWorkoutExercise : IEndpoint
{
	// NOTE: this is currently useless

	public static async Task<Results<NoContent, NotFound<string>, ValidationProblem, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int index,
		[FromBody] EditWorkoutExerciseRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var workoutIdTyped = new Id<Workout>(workoutId);
		var workout = await dataContext.Workouts.Include(workout => workout.Users)
			.Include(workout => workout.Exercises)
			.FirstOrDefaultAsync(workout => workout.Id == workoutIdTyped, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return TypedResults.NotFound("Workout not found.");
		if (!httpContext.User.CanModifyOrDelete(workout.Users)) return TypedResults.Forbid();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == index);
		if (exercise is null) return TypedResults.NotFound("Exercise not found.");

		// can't reasonably update the index, it's not beneficial really anyway
		// exercise.Index = request.Index;

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{index:int}", Handler);

		return builder;
	}
}
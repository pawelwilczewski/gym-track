using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts.Exercises.DisplayOrder;

internal sealed class EditWorkoutExerciseDisplayOrder : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromBody] EditDisplayOrderRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var workoutIdTyped = new Id<Workout>(workoutId);
		var workout = await dataContext.Workouts
			.Include(workout => workout.Users)
			.Include(workout => workout.Exercises)
			.FirstOrDefaultAsync(workout => workout.Id == workoutIdTyped, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return TypedResults.NotFound("Workout not found.");
		if (!httpContext.User.CanModifyOrDelete(workout.Users)) return TypedResults.Forbid();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == exerciseIndex);
		if (exercise is null) return TypedResults.NotFound("Exercise not found.");

		exercise.DisplayOrder = request.DisplayOrder;

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("", Handler);
		return builder;
	}
}
using Api.Dtos;
using Application.Persistence;
using Domain.Common;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Tracking;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Tracking.Workouts;

internal sealed class CreateTrackedWorkout : IEndpoint
{
	public static async Task<Results<Created, BadRequest<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromBody] CreateTrackedWorkoutRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var workoutId = new Id<Workout>(request.WorkoutId);

		var workout = await dataContext.Workouts
			.AsNoTracking()
			.Include(workout => workout.Users)
			.FirstOrDefaultAsync(workout => workout.Id == workoutId, cancellationToken);

		if (workout is null) return TypedResults.BadRequest("Workout not found.");
		if (!httpContext.User.CanAccess(workout.Users)) return TypedResults.Forbid();

		var trackedWorkout = new TrackedWorkout(
			workoutId,
			httpContext.User.GetUserId(),
			request.PerformedAt,
			request.Duration);

		dataContext.TrackedWorkouts.Add(trackedWorkout);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.Created($"{httpContext.Request.Path}/{trackedWorkout.Id}");
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler);
		return builder;
	}
}
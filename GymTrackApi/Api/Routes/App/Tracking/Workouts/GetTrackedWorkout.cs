using Api.Dtos;
using Application.Persistence;
using Domain.Common;
using Domain.Models;
using Domain.Models.Tracking;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Tracking.Workouts;

internal sealed class GetTrackedWorkout : IEndpoint
{
	public static async Task<Results<Ok<GetTrackedWorkoutResponse>, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid trackedWorkoutId,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var id = new Id<TrackedWorkout>(trackedWorkoutId);
		var trackedWorkout = await dataContext.TrackedWorkouts
			.AsNoTracking()
			.Include(trackedWorkout => trackedWorkout.User)
			.FirstOrDefaultAsync(workout => workout.Id == id, cancellationToken);

		if (trackedWorkout is null) return TypedResults.NotFound("Tracked Workout not found.");
		if (httpContext.User.GetUserId() != trackedWorkout.UserId) return TypedResults.Forbid();

		return TypedResults.Ok(new GetTrackedWorkoutResponse(
			trackedWorkoutId,
			trackedWorkout.WorkoutId.Value,
			trackedWorkout.PerformedAt,
			trackedWorkout.Duration));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{trackedWorkoutId:guid}", Handler);
		return builder;
	}
}
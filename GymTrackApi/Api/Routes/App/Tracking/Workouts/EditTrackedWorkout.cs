using Api.Dtos;
using Application.Persistence;
using Domain.Common;
using Domain.Models;
using Domain.Models.Tracking;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Tracking.Workouts;

internal sealed class EditTrackedWorkout : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid trackedWorkoutId,
		[FromBody] EditTrackedWorkoutRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var id = new Id<TrackedWorkout>(trackedWorkoutId);
		var trackedWorkout = await dataContext.TrackedWorkouts
			.Include(trackedWorkout => trackedWorkout.User)
			.FirstOrDefaultAsync(trackedWorkout => trackedWorkout.Id == id, cancellationToken)
			.ConfigureAwait(false);

		if (trackedWorkout is null) return TypedResults.NotFound("Tracked Workout not found.");
		if (trackedWorkout.UserId != httpContext.User.GetUserId()) return TypedResults.Forbid();

		trackedWorkout.PerformedAt = request.PerformedAt;
		trackedWorkout.Duration = request.Duration;

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{trackedWorkoutId:guid}", Handler);
		return builder;
	}
}
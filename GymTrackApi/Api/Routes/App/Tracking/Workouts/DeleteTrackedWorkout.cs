using Application.Persistence;
using Domain.Common;
using Domain.Models;
using Domain.Models.Tracking;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Tracking.Workouts;

internal sealed class DeleteTrackedWorkout : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid trackedWorkoutId,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var id = new Id<TrackedWorkout>(trackedWorkoutId);

		var trackedWorkout = await dataContext.TrackedWorkouts
			.FirstOrDefaultAsync(trackedWorkout => trackedWorkout.Id == id, cancellationToken)
			.ConfigureAwait(false);

		if (trackedWorkout == null) return TypedResults.NotFound("Tracked Workout not found");
		if (httpContext.User.GetUserId() != trackedWorkout.UserId) return TypedResults.Forbid();

		dataContext.TrackedWorkouts.Remove(trackedWorkout);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{trackedWorkoutId:guid}", Handler);
		return builder;
	}
}
using Api.Dtos;
using Application.Persistence;
using Domain.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Tracking.Workouts;

internal sealed class GetTrackedWorkouts : IEndpoint
{
	public static async Task<Results<Ok<List<GetTrackedWorkoutResponse>>, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var userId = httpContext.User.GetUserId();
		var trackedWorkouts = dataContext.TrackedWorkouts
			.AsNoTracking()
			.Where(trackedWorkout => trackedWorkout.UserId == userId);

		return TypedResults.Ok(await trackedWorkouts.Select(trackedWorkout => new GetTrackedWorkoutResponse(
				trackedWorkout.Id.Value,
				trackedWorkout.WorkoutId.Value,
				trackedWorkout.PerformedAt,
				trackedWorkout.Duration))
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("", Handler);
		return builder;
	}
}
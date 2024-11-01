using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts;

internal sealed class DeleteWorkout : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid id,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var workoutId = new Id<Workout>(id);
		var workout = await dataContext.Workouts.Include(workout => workout.Users)
			.FirstOrDefaultAsync(workout => workout.Id == workoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return TypedResults.NotFound("Workout not found.");
		if (!httpContext.User.CanModifyOrDelete(workout.Users)) return TypedResults.Forbid();

		dataContext.Workouts.Remove(workout);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{id:guid}", Handler);
		return builder;
	}
}
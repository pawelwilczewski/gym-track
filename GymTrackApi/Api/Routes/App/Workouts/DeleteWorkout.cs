using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts;

internal sealed class DeleteWorkout : IEndpoint
{
	public static async Task<Results<Ok, NotFound<string>, BadRequest<string>, UnauthorizedHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid id,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var workoutId = new Id<Workout>(id);
		var workout = await dataContext.Workouts.Include(workout => workout.Users)
			.FirstOrDefaultAsync(workout => workout.Id == workoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null || !httpContext.User.CanModifyOrDelete(workout.Users, out _))
		{
			return TypedResults.NotFound("Workout not found.");
		}

		dataContext.Workouts.Remove(workout);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.Ok();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{id:guid}", Handler);
		return builder;
	}
}
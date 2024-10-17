using System.Diagnostics;
using Application.Persistence;
using Domain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Workout;

internal sealed class DeleteWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("/{id:guid}", Delete)
			.RequireAuthorization();

		return builder;

		async Task<Results<Ok, NotFound, UnauthorizedHttpResult, BadRequest<string>>> Delete(
			HttpContext httpContext,
			Guid id,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken)
		{
			var workoutId = new Id<Domain.Models.Workout.Workout>(id);
			var workout = await dataContext.Workouts
				.Include(workout => workout.UserWorkouts)
				.FirstOrDefaultAsync(
					workout => workout.Id == workoutId,
					cancellationToken)
				.ConfigureAwait(false);

			if (workout is null) return TypedResults.NotFound();

			switch (workout.CanDeleteOrModify(httpContext.User))
			{
				case Domain.Models.Workout.Workout.CanModifyResult.Yes:
				{
					dataContext.Workouts.Remove(workout);
					await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
					return TypedResults.Ok();
				}
				case Domain.Models.Workout.Workout.CanModifyResult.Unauthorized:   return TypedResults.Unauthorized();
				case Domain.Models.Workout.Workout.CanModifyResult.NotFound:       return TypedResults.NotFound();
				case Domain.Models.Workout.Workout.CanModifyResult.ProhibitShared: return TypedResults.BadRequest("Can't delete shared workout");
				default:                                                           throw new UnreachableException();
			}
		}
	}
}
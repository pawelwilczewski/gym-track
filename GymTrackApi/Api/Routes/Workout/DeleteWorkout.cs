using Api.Common;
using Application.Persistence;
using Domain.Models;
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

		async Task<IResult> Delete(
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

			return await workout.CanDeleteOrModify(httpContext.User)
				.ToResult(async () =>
				{
					dataContext.Workouts.Remove(workout);
					await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
					return TypedResults.Ok();
				});
		}
	}
}
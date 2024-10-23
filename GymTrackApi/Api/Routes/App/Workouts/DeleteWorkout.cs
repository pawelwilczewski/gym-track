using Api.Common;
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
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("/{id:guid}", async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>> (
			HttpContext httpContext,
			Guid id,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var workoutId = new Id<Workout>(id);
			var workout = await dataContext.Workouts
				.Include(workout => workout.Users)
				.FirstOrDefaultAsync(
					workout => workout.Id == workoutId,
					cancellationToken)
				.ConfigureAwait(false);

			if (workout is null) return TypedResults.NotFound();
			if (!httpContext.User.CanModifyOrDelete(workout.Users, out var reason)) return reason.ToResult();

			dataContext.Workouts.Remove(workout);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			return TypedResults.Ok();
		});

		return builder;
	}
}
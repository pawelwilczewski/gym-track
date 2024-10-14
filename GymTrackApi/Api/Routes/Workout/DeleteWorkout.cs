using Api.Authorization;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Workout;

internal sealed class DeleteWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("/delete/{id:guid}", Delete)
			.RequireAuthorization();

		return builder;

		async Task<Results<Ok, NotFound, UnauthorizedHttpResult, BadRequest<string>>> Delete(
			HttpContext httpContext,
			Guid id,
			[FromServices] IDataContext dataContext,
			[FromServices] UserManager<User> userManager,
			[FromServices] RoleManager<Role> roleManager,
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

			// in case there are no user workouts associated,
			// this is a template workout - can be only deleted by admins
			if (workout.UserWorkouts.Count <= 0
				&& !httpContext.User.IsInRole(Roles.ADMINISTRATOR))
				return TypedResults.Unauthorized();

			try
			{
				dataContext.Workouts.Remove(workout);
				await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (InvalidOperationException) // deletion could be restricted
			{
				return TypedResults.BadRequest(
					"Delete all UserWorkout's referencing this workout, before deleting Workout! (rethink this?)");
			}

			return TypedResults.Ok();
		}
	}
}
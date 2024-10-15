using Application.Persistence;
using Domain.Common;
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
				.ThenInclude(userWorkout => userWorkout.User)
				.FirstOrDefaultAsync(
					workout => workout.Id == workoutId,
					cancellationToken)
				.ConfigureAwait(false);

			if (workout is null) return TypedResults.NotFound();

			switch (workout.UserWorkouts)
			{
				// in case there are no user workouts associated,
				// this is a template workout - can be only deleted by admins
				case []:
				{
					if (!httpContext.User.IsInRole(Role.ADMINISTRATOR)) return TypedResults.Unauthorized();

					break;
				}
				case [var userWorkout]:
				{
					if (userWorkout.UserId != httpContext.User.GetUserId()) return TypedResults.NotFound();

					break;
				}
				case [..]: return TypedResults.BadRequest("Cannot delete shared workout");
			}

			dataContext.Workouts.Remove(workout);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			return TypedResults.Ok();
		}
	}
}
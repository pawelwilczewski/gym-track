using Api.Authorization;
using Api.Dtos;
using Application.Persistence;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Workout;

internal sealed class Create : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/create", async (
				HttpContext httpContext,
				[FromBody] CreateWorkout createWorkout,
				[FromServices] IDataContext dataContext,
				[FromServices] UserManager<User> userManager,
				[FromServices] RoleManager<Role> roleManager,
				CancellationToken cancellationToken) =>
			{
				if (httpContext.User.IsInRole(Roles.ADMINISTRATOR))
				{
					dataContext.Workouts.Add(new Domain.Models.Workout.Workout
					{
						Name = createWorkout.Name
					});
				}
				else
				{
					var user = (await userManager.GetUserAsync(httpContext.User))!;

					var workout = new Domain.Models.Workout.Workout
					{
						Name = createWorkout.Name
					};

					var userWorkout = new UserWorkout
					{
						User = user,
						Workout = workout
					};

					workout.UserWorkouts.Add(userWorkout);

					dataContext.UserWorkouts.Add(userWorkout);
				}

				await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
				return TypedResults.Ok();
			})
			.RequireAuthorization();

		return builder;
	}
}
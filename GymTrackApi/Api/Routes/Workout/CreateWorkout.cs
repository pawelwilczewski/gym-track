using Api.Dtos;
using Application.Persistence;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Workout;

internal sealed class CreateWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/create", async (
				HttpContext httpContext,
				[FromBody] CreateWorkoutRequest createWorkout,
				[FromServices] IDataContext dataContext,
				CancellationToken cancellationToken) =>
			{
				var workout = httpContext.User.IsInRole(Role.ADMINISTRATOR)
					? Domain.Models.Workout.Workout.CreateDefault(createWorkout.Name)
					: Domain.Models.Workout.Workout.CreateForUser(createWorkout.Name, httpContext.User);

				dataContext.Workouts.Add(workout);
				await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

				return TypedResults.Ok();
			})
			.RequireAuthorization();

		return builder;
	}
}
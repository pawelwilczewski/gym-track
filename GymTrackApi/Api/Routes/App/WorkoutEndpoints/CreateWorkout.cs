using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.WorkoutEndpoints;

internal sealed class CreateWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/create", async Task<Results<Ok, BadRequest<string>>> (
			HttpContext httpContext,
			[FromBody] CreateWorkoutRequest request,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			if (!Name.TryCreate(request.Name, out var name, out var invalid))
			{
				return TypedResults.BadRequest(invalid.Error);
			}

			var workout = httpContext.User.IsInRole(Role.ADMINISTRATOR)
				? Workout.CreateForEveryone(name)
				: Workout.CreateForUser(name, httpContext.User);

			dataContext.Workouts.Add(workout);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			return TypedResults.Ok();
		});

		return builder;
	}
}
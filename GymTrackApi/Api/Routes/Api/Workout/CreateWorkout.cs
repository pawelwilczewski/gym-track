using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Api.Workout;

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
				if (Name.TryCreate(request.Name, out var name) is TextValidationResult.Invalid invalid)
				{
					return TypedResults.BadRequest(invalid.Error);
				}

				var workout = httpContext.User.IsInRole(Role.ADMINISTRATOR)
					? Domain.Models.Workout.Workout.CreateForEveryone(name)
					: Domain.Models.Workout.Workout.CreateForUser(name, httpContext.User);

				dataContext.Workouts.Add(workout);
				await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

				return TypedResults.Ok();
			});

		return builder;
	}
}
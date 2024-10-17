using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Workout;

internal sealed class CreateWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/create", Create)
			.RequireAuthorization();

		return builder;

		async Task<Results<Ok, BadRequest<string>>> Create(
			HttpContext httpContext,
			[FromBody] CreateWorkoutRequest createWorkout,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken)
		{
			if (Name.TryCreate(createWorkout.Name, out var name) is TextValidationResult.Invalid invalid)
			{
				return TypedResults.BadRequest(invalid.Error);
			}

			var workout = httpContext.User.IsInRole(Role.ADMINISTRATOR)
				? Domain.Models.Workout.Workout.CreateForEveryone(name)
				: Domain.Models.Workout.Workout.CreateForUser(name, httpContext.User);

			dataContext.Workouts.Add(workout);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			return TypedResults.Ok();
		}
	}
}
using Api.Common;
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts;

internal sealed class CreateWorkout : IEndpoint
{
	public static async Task<Results<Created, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromBody] CreateWorkoutRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		if (!Name.TryCreate(request.Name, out var name, out var error))
		{
			return error.ToValidationProblem("Name");
		}

		var workout = httpContext.User.IsInRole(Role.ADMINISTRATOR)
			? Workout.CreateForEveryone(name)
			: Workout.CreateForUser(name, httpContext.User);

		dataContext.Workouts.Add(workout);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.Created($"{httpContext.Request.Path}/{workout.Id}");
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler);
		return builder;
	}
}
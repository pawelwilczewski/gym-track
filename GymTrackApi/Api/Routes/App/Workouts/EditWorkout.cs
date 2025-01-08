using Api.Common;
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts;

internal sealed class EditWorkout : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ValidationProblem, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromBody] EditWorkoutRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var typedWorkoutId = new Id<Workout>(workoutId);
		var workout = await dataContext.Workouts
			.Include(workout => workout.Users)
			.FirstOrDefaultAsync(workout => workout.Id == typedWorkoutId, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return TypedResults.NotFound("Workout not found.");
		if (!httpContext.User.CanModifyOrDelete(workout.Users)) return TypedResults.Forbid();

		if (!workout.Name.TrySet(request.Name, out var error))
		{
			return error.ToValidationProblem("Name");
		}

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{workoutId:guid}", Handler);
		return builder;
	}
}
using Domain.Models;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts;

internal sealed class DeleteWorkout : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var typedWorkoutId = new Id<Workout>(workoutId);

		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{workoutId:guid}", Handler);
		return builder;
	}
}
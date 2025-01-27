using Application.Tracking.TrackedWorkout.Commands;
using Domain.Common;
using Domain.Models;
using Domain.Models.Tracking;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Tracking.Workouts;

using ResultType = Results<NoContent, NotFound>;

internal sealed class DeleteTrackedWorkout : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid trackedWorkoutId,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(
				new DeleteTrackedWorkoutCommand(
					new Id<TrackedWorkout>(trackedWorkoutId),
					httpContext.User.GetUserId()),
				cancellationToken)
			.ConfigureAwait(false);

		return result.Match<ResultType>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{trackedWorkoutId:guid}", Handler);
		return builder;
	}
}
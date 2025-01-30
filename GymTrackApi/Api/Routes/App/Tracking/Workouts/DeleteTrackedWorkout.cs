using Application.Tracking.TrackedWorkout.Commands;
using Domain.Common;
using Domain.Models.Tracking;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Tracking.Workouts;

using ResultType = Results<NoContent, NotFound>;

internal sealed class DeleteTrackedWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{trackedWorkoutId:guid}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid trackedWorkoutId,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(
					new DeleteTrackedWorkoutCommand(
						TrackedWorkoutId.From(trackedWorkoutId),
						httpContext.User.GetUserId()),
					cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
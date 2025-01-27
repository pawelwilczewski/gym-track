using Api.Dtos;
using Application.Tracking.TrackedWorkout.Commands;
using Domain.Common;
using Domain.Models;
using Domain.Models.Tracking;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Tracking.Workouts;

using ResultType = Results<NoContent, NotFound>;

internal sealed class UpdateTrackedWorkout : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid trackedWorkoutId,
		[FromBody] UpdateTrackedWorkoutRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(
				new UpdateTrackedWorkoutCommand(
					new Id<TrackedWorkout>(trackedWorkoutId),
					request.PerformedAt,
					request.Duration,
					httpContext.User.GetUserId()),
				cancellationToken)
			.ConfigureAwait(false);

		return result.Match<ResultType>(
			success => TypedResults.NotFound(),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{trackedWorkoutId:guid}", Handler);
		return builder;
	}
}
using Application.Tracking.TrackedWorkout.Dtos;
using Application.Tracking.TrackedWorkout.Queries;
using Domain.Common;
using Domain.Models.Tracking;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Tracking.Workouts;

using ResultType = Results<Ok<GetTrackedWorkoutResponse>, NotFound>;

internal sealed class GetTrackedWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{trackedWorkoutId:guid}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid trackedWorkoutId,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(new GetTrackedWorkoutQuery(
					TrackedWorkoutId.From(trackedWorkoutId),
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.Ok(success.Value),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
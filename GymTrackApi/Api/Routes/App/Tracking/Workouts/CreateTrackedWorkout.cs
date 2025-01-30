using Api.Dtos;
using Application.Tracking.TrackedWorkout.Commands;
using Domain.Common;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Tracking.Workouts;

using ResultType = Results<Created, NotFound>;

internal sealed class CreateTrackedWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", async Task<ResultType> (
			HttpContext httpContext,
			[FromBody] CreateTrackedWorkoutRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(new CreateTrackedWorkoutCommand(
					WorkoutId.From(request.WorkoutId),
					request.PerformedAt,
					request.Duration,
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.Created($"{httpContext.Request.Path}/{success.Value.Id}"),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
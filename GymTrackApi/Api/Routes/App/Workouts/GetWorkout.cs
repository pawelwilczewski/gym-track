using Application.Workout.Dtos;
using Application.Workout.Queries;
using Domain.Common;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts;

using ResultType = Results<Ok<GetWorkoutResponse>, NotFound>;

internal sealed class GetWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{workoutId:guid}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid workoutId,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(new GetWorkoutQuery(
					WorkoutId.From(workoutId),
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.Ok(success.Value),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
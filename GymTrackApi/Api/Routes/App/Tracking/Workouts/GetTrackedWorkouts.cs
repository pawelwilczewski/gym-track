using Application.Tracking.TrackedWorkout.Dtos;
using Application.Tracking.TrackedWorkout.Queries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Tracking.Workouts;

using ResultType = Ok<List<GetTrackedWorkoutResponse>>;

internal sealed class GetTrackedWorkouts : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("", async Task<ResultType> (
			HttpContext httpContext,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(
					new GetTrackedWorkoutsQuery(
						httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return TypedResults.Ok(result.Value);
		});

		return builder;
	}
}
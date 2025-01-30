using Application.Workout.Dtos;
using Application.Workout.Queries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts;

using ResultType = Ok<List<GetWorkoutResponse>>;

internal sealed class GetWorkouts : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("", async Task<ResultType> (
			HttpContext httpContext,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(new GetWorkoutsQuery(
						httpContext.User.GetUserId()),
					cancellationToken)
				.ConfigureAwait(false);

			return TypedResults.Ok(result.Value);
		});

		return builder;
	}
}
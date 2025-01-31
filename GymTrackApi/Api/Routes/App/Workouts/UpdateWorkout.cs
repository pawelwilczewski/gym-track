using Api.Common;
using Api.Dtos;
using Application.Workout.Commands;
using Domain.Common;
using Domain.Common.ValueObjects;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts;

using ResultType = Results<NoContent, NotFound, ValidationProblem>;

internal sealed class UpdateWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{workoutId:guid}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid workoutId,
			[FromBody] UpdateWorkoutRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var nameOrError = Name.TryFrom(request.Name);
			if (!nameOrError.IsSuccess)
			{
				return nameOrError.Error.ToValidationProblem(nameof(request.Name));
			}

			var result = await sender.Send(new UpdateWorkoutCommand(
					WorkoutId.From(workoutId),
					nameOrError.ValueObject,
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
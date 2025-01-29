using Api.Dtos;
using Application.Workout.Exercise.DisplayOrder.Commands;
using Domain.Common;
using Domain.Models;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts.Exercises.DisplayOrder;

using ResultType = Results<NoContent, NotFound>;

internal sealed class UpdateWorkoutExerciseDisplayOrder : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromBody] UpdateDisplayOrderRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(new UpdateWorkoutExerciseDisplayOrderCommand(
				new Id<Workout>(workoutId),
				exerciseIndex,
				request.DisplayOrder,
				httpContext.User.GetUserId()), cancellationToken)
			.ConfigureAwait(false);

		return result.Match<ResultType>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("", Handler);
		return builder;
	}
}
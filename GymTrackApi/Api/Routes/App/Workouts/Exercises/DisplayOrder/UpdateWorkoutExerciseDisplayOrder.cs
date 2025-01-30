using Api.Dtos;
using Application.Workout.Exercise.DisplayOrder.Commands;
using Domain.Common;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts.Exercises.DisplayOrder;

using ResultType = Results<NoContent, NotFound>;

internal sealed class UpdateWorkoutExerciseDisplayOrder : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid workoutId,
			[FromRoute] int exerciseIndex,
			[FromBody] UpdateDisplayOrderRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(new UpdateWorkoutExerciseDisplayOrderCommand(
					WorkoutId.From(workoutId),
					WorkoutExerciseIndex.From(exerciseIndex),
					request.DisplayOrder,
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
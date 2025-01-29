using Application.Workout.Exercise.Commands;
using Domain.Common;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts.Exercises;

using ResultType = Results<NoContent, NotFound>;

internal sealed class DeleteWorkoutExercise : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(new DeleteWorkoutExerciseCommand(
				WorkoutId.From(workoutId),
				exerciseIndex,
				httpContext.User.GetUserId()), cancellationToken)
			.ConfigureAwait(false);

		return result.Match<ResultType>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{exerciseIndex:int}", Handler);
		return builder;
	}
}
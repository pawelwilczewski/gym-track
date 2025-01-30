using Application.Workout.Exercise.Dtos;
using Application.Workout.Exercise.Queries;
using Domain.Common;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts.Exercises;

using ResultType = Results<Ok<GetWorkoutExerciseResponse>, NotFound>;

internal sealed class GetWorkoutExercise : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{exerciseIndex:int}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid workoutId,
			[FromRoute] int exerciseIndex,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(new GetWorkoutExerciseQuery(
					WorkoutId.From(workoutId),
					exerciseIndex,
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.Ok(success.Value),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
using Application.Workout.Exercise.Set.Dtos;
using Application.Workout.Exercise.Set.Queries;
using Domain.Common;
using Domain.Models;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts.Exercises.Sets;

using ResultType = Results<Ok<GetWorkoutExerciseSetResponse>, NotFound>;

internal sealed class GetWorkoutExerciseSet : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromRoute] int setIndex,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(new GetWorkoutExerciseSetQuery(
				new Id<Workout>(workoutId),
				exerciseIndex,
				setIndex,
				httpContext.User.GetUserId()), cancellationToken)
			.ConfigureAwait(false);

		return result.Match<ResultType>(
			success => TypedResults.Ok(success.Value),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{setIndex:int}", Handler);
		return builder;
	}
}
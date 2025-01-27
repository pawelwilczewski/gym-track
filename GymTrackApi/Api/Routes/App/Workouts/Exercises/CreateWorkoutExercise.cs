using Api.Dtos;
using Application.Workout.Exercise.Commands;
using Domain.Common;
using Domain.Models;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts.Exercises;

using ResultType = Results<Created, NotFound>;

internal sealed class CreateWorkoutExercise : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromBody] CreateWorkoutExerciseRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(new CreateWorkoutExerciseCommand(
				new Id<Workout>(workoutId),
				new Id<ExerciseInfo>(request.ExerciseInfoId),
				httpContext.User.GetUserId()), cancellationToken)
			.ConfigureAwait(false);

		return result.Match<ResultType>(
			success => TypedResults.Created($"{httpContext.Request.Path}/{success.Value.Index}"),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler);
		return builder;
	}
}
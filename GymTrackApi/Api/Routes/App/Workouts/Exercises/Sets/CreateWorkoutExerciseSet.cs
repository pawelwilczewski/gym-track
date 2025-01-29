using Api.Common;
using Api.Dtos;
using Application.Workout.Exercise.Set.Commands;
using Domain.Common;
using Domain.Models;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts.Exercises.Sets;

using ResultType = Results<Created, NotFound, ValidationProblem>;

internal sealed class CreateWorkoutExerciseSet : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromBody] CreateWorkoutExerciseSetRequest request,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var repsOrError = Reps.TryFrom(request.Reps);
		if (!repsOrError.IsSuccess) return repsOrError.Error.ToValidationProblem(nameof(request.Reps));

		var result = await sender.Send(new CreateWorkoutExerciseSetCommand(
					WorkoutId.From(workoutId),
					exerciseIndex,
					request.Metric,
					repsOrError.ValueObject,
					httpContext.User.GetUserId()),
				cancellationToken)
			.ConfigureAwait(false);

		return result.Match<ResultType>(
			success => TypedResults.Created($"{httpContext.Request.Path}/{success.Value.Index}"),
			notFound => TypedResults.NotFound(),
			validationError => validationError.ToValidationProblem(""));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler);
		return builder;
	}
}
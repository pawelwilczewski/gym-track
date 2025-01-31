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

using ResultType = Results<NoContent, NotFound, ValidationProblem>;

internal sealed class UpdateWorkoutExerciseSet : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{setIndex:int}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid workoutId,
			[FromRoute] int exerciseIndex,
			[FromRoute] int setIndex,
			[FromBody] UpdateWorkoutExerciseSetRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var repsOrError = Reps.TryFrom(request.Reps);
			if (!repsOrError.IsSuccess) return repsOrError.Error.ToValidationProblem(nameof(request.Reps));

			var result = await sender.Send(new UpdateWorkoutExerciseSetCommand(
						WorkoutId.From(workoutId),
						WorkoutExerciseIndex.From(exerciseIndex),
						WorkoutExerciseSetIndex.From(setIndex),
						request.Metric,
						repsOrError.ValueObject,
						httpContext.User.GetUserId()),
					cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				notFound => TypedResults.NotFound(),
				validationError => validationError.ToValidationProblem(""));
		});

		return builder;
	}
}
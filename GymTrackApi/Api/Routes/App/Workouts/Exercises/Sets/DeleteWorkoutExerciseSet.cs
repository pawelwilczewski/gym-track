using Application.Workout.Exercise.Set.Commands;
using Domain.Common;
using Domain.Models.Workout;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.Workouts.Exercises.Sets;

using ResultType = Results<NoContent, NotFound>;

internal sealed class DeleteWorkoutExerciseSet : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromRoute] int setIndex,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(new DeleteWorkoutExerciseSetCommand(
				WorkoutId.From(workoutId),
				WorkoutExerciseIndex.From(exerciseIndex),
				setIndex,
				httpContext.User.GetUserId()), cancellationToken)
			.ConfigureAwait(false);

		return result.Match<ResultType>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{setIndex:int}", Handler);
		return builder;
	}
}
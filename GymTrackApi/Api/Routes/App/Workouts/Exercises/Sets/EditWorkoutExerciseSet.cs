using Api.Common;
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts.Exercises.Sets;

internal sealed class EditWorkoutExerciseSet : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromRoute] int index,
		[FromBody] EditWorkoutExerciseSetRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		if (!PositiveCount.TryCreate(request.Reps, out var repsCount)) return ValidationErrors.NonPositiveCount("Reps");

		var workoutIdTyped = new Id<Workout>(workoutId);
		var workout = await dataContext.Workouts.Include(workout => workout.Users)
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.ExerciseInfo)
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.Sets)
			.FirstOrDefaultAsync(workout => workout.Id == workoutIdTyped, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return TypedResults.NotFound("Workout not found.");
		if (!httpContext.User.CanModifyOrDelete(workout.Users)) return TypedResults.Forbid();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == exerciseIndex);
		if (exercise is null) return TypedResults.NotFound("Exercise not found.");

		var set = exercise.Sets.FirstOrDefault(set => set.Index == index);
		if (set is null) return TypedResults.NotFound("Set not found.");

		if (!exercise.ExerciseInfo.AllowedMetricTypes.HasFlag(request.Metric.Type))
		{
			return request.Metric.NotAllowedValidationProblem();
		}

		set.Metric = request.Metric;
		set.Reps = repsCount;

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{index:int}", Handler);
		return builder;
	}
}
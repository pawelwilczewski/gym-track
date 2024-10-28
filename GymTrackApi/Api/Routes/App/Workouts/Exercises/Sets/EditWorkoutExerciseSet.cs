using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts.Exercises.Sets;

internal sealed class EditWorkoutExerciseSet : IEndpoint
{
	public static async Task<Results<Ok, NotFound<string>, BadRequest<string>>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromRoute] int index,
		[FromBody] EditWorkoutExerciseSetRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var workoutIdTyped = new Id<Workout>(workoutId);
		var workout = await dataContext.Workouts.Include(workout => workout.Users)
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.ExerciseInfo)
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.Sets)
			.FirstOrDefaultAsync(workout => workout.Id == workoutIdTyped, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null || !httpContext.User.CanAccess(workout.Users))
		{
			return TypedResults.NotFound("Workout not found.");
		}

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == index);
		if (exercise is null) return TypedResults.NotFound("Exercise not found.");

		var set = exercise.Sets.FirstOrDefault(set => set.Index == index);
		if (set is null) return TypedResults.NotFound("Set not found.");

		if (!exercise.ExerciseInfo.AllowedMetricTypes.HasFlag(request.Metric.Type))
		{
			return TypedResults.BadRequest($"Invalid metric type: {request.Metric.Type}. "
				+ $"Accepted types are: {exercise.ExerciseInfo.AllowedMetricTypes}.");
		}

		set.Metric = request.Metric;
		set.Reps = request.Reps;

		return TypedResults.Ok();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{index:int}", Handler);
		return builder;
	}
}
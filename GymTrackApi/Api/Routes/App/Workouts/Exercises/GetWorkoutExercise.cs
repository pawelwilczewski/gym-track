using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts.Exercises;

internal sealed class GetWorkoutExercise : IEndpoint
{
	public static async Task<Results<Ok<GetWorkoutExerciseResponse>, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int index,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var workoutIdTyped = new Id<Workout>(workoutId);
		var workout = await dataContext.Workouts.AsNoTracking()
			.Include(workout => workout.Users)
			.Include(workout => workout.Exercises)
			.ThenInclude(exercise => exercise.Sets)
			.FirstOrDefaultAsync(workout => workout.Id == workoutIdTyped, cancellationToken);

		if (workout is null) return TypedResults.NotFound("Workout not found.");
		if (!httpContext.User.CanAccess(workout.Users)) return TypedResults.Forbid();

		var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == index);
		if (exercise is null) return TypedResults.NotFound("Exercise not found.");

		return TypedResults.Ok(new GetWorkoutExerciseResponse(
			exercise.Index,
			exercise.ExerciseInfoId.Value,
			exercise.Sets.Select(set => new WorkoutExerciseSetKey(workoutId, index, set.Index))
				.ToList()));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{index:int}", Handler);

		return builder;
	}
}
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts;

internal sealed class GetWorkout : IEndpoint
{
	public static async Task<Results<Ok<GetWorkoutResponse>, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var typedWorkoutId = new Id<Workout>(workoutId);
		var workout = await dataContext.Workouts.AsNoTracking()
			.Include(workout => workout.Users)
			.Include(workout => workout.Exercises)
			.FirstOrDefaultAsync(workout => workout.Id == typedWorkoutId, cancellationToken);

		if (workout is null) return TypedResults.NotFound("Workout not found.");
		if (!httpContext.User.CanAccess(workout.Users)) return TypedResults.Forbid();

		return TypedResults.Ok(new GetWorkoutResponse(
			workout.Id.Value,
			workout.Name.ToString(),
			workout.Exercises.Select(exercise => new WorkoutExerciseKey(typedWorkoutId.Value, exercise.Index))
				.ToList()));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{workoutId:guid}", Handler);
		return builder;
	}
}
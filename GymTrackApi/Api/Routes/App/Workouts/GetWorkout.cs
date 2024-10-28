using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts;

internal sealed class GetWorkout : IEndpoint
{
	public static async Task<Results<Ok<GetWorkoutResponse>, NotFound>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid id,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var workoutId = new Id<Workout>(id);
		var workout = await dataContext.Workouts.AsNoTracking()
			.Include(workout => workout.Users)
			.Include(workout => workout.Exercises)
			.FirstOrDefaultAsync(workout => workout.Id == workoutId, cancellationToken);

		if (workout is null || !httpContext.User.CanAccess(workout.Users)) return TypedResults.NotFound();

		return TypedResults.Ok(new GetWorkoutResponse(
			workout.Name.ToString(),
			workout.Exercises.Select(exercise => new WorkoutExerciseKey(workoutId.Value, exercise.Index))
				.ToList()));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{id:guid}", Handler);
		return builder;
	}
}
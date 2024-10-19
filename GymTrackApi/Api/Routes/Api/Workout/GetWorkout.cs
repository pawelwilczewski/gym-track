using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Api.Workout;

internal sealed class GetWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("/{id:guid}", async Task<Results<Ok<GetWorkoutResponse>, NotFound>> (
				HttpContext httpContext,
				[FromRoute] Guid id,
				[FromServices] IDataContext dataContext,
				CancellationToken cancellationToken) =>
			{
				var workoutId = new Id<Domain.Models.Workout.Workout>(id);
				var workout = await dataContext.Workouts
					.Include(workout => workout.Users)
					.Include(workout => workout.Exercises)
					.FirstOrDefaultAsync(workout => workout.Id == workoutId, cancellationToken);

				if (workout is null || !httpContext.User.CanAccessWorkout(workout.Users)) return TypedResults.NotFound();

				return TypedResults.Ok(new GetWorkoutResponse(
					workout.Name.ToString(),
					workout.Exercises.Select(exercise => new ExerciseKey(exercise.Index)).ToList()));
			})
			.RequireAuthorization();

		return builder;
	}
}
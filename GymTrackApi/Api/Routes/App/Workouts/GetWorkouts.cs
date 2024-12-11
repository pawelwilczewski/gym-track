using Api.Dtos;
using Application.Persistence;
using Domain.Common;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts;

internal sealed class GetWorkouts : IEndpoint
{
	public static async Task<Results<Ok<List<GetWorkoutResponse>>, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var userId = httpContext.User.GetUserId();
		var isAdmin = httpContext.User.IsInRole(Role.ADMINISTRATOR);
		var workouts = dataContext.Workouts
			.Where(workout => workout.Users.Count <= 0 || isAdmin || workout.Users.Any(user => user.UserId == userId))
			.AsNoTracking();
		var workoutsResponse = workouts.Select(workout => new GetWorkoutResponse(
			workout.Id.Value,
			workout.Name.ToString(),
			workout.Exercises.Select(exercise => new WorkoutExerciseKey(workout.Id.Value, exercise.Index)).ToList()));
		return TypedResults.Ok(await workoutsResponse
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("", Handler);
		return builder;
	}
}
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts.Exercises;

internal sealed class DeleteWorkoutExercise : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{index:int}", async Task<Results<Ok, NotFound<string>>> (
			HttpContext httpContext,
			[FromRoute] Guid workoutId,
			[FromRoute] int index,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var workoutIdTyped = new Id<Workout>(workoutId);
			var workout = await dataContext.Workouts
				.Include(workout => workout.Users)
				.Include(workout => workout.Exercises)
				.FirstOrDefaultAsync(
					workout => workout.Id == workoutIdTyped,
					cancellationToken)
				.ConfigureAwait(false);

			if (workout is null || !httpContext.User.CanModifyOrDelete(workout.Users, out _))
			{
				return TypedResults.NotFound("Workout not found.");
			}

			var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == index);
			if (exercise is null) return TypedResults.NotFound("Exercise not found.");

			workout.Exercises.Remove(exercise);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			return TypedResults.Ok();
		});

		return builder;
	}
}
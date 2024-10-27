using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts.Exercises;

internal sealed class CreateWorkoutExercise : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", async Task<Results<Ok, NotFound<string>>> (
			HttpContext httpContext,
			[FromRoute] Guid workoutId,
			[FromBody] CreateWorkoutExerciseRequest request,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var workoutIdTyped = new Id<Workout>(workoutId);
			var workout = await dataContext.Workouts
				.Include(workout => workout.Users)
				.FirstOrDefaultAsync(workout => workout.Id == workoutIdTyped, cancellationToken)
				.ConfigureAwait(false);

			if (workout is null || !httpContext.User.CanModifyOrDelete(workout.Users, out _))
			{
				return TypedResults.NotFound("Workout not found.");
			}

			var exerciseInfoId = new Id<ExerciseInfo>(request.ExerciseInfoId);
			var exerciseInfo = await dataContext.ExerciseInfos
				.AsNoTracking()
				.Include(exerciseInfo => exerciseInfo.Users)
				.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == exerciseInfoId, cancellationToken)
				.ConfigureAwait(false);

			if (exerciseInfo is null || !httpContext.User.CanAccess(exerciseInfo.Users))
			{
				return TypedResults.NotFound("Exercise not found.");
			}

			var exercise = new Workout.Exercise(workoutIdTyped, request.Index, exerciseInfoId);

			workout.Exercises.Add(exercise);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			return TypedResults.Ok();
		});

		return builder;
	}
}
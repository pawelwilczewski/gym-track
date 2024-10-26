using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.Workouts.Exercises.Sets;

internal sealed class CreateWorkoutExerciseSet : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", async Task<Results<Ok, NotFound<string>, BadRequest<string>>> (
			HttpContext httpContext,
			[FromRoute] Guid workoutId,
			[FromRoute] int exerciseIndex,
			[FromBody] CreateWorkoutExerciseSetRequest request,
			[FromServices] IDataContext dataContext,
			[FromServices] UserManager<User> userManager,
			CancellationToken cancellationToken) =>
		{
			var workoutIdTyped = new Id<Workout>(workoutId);
			var workout = await dataContext.Workouts
				.Include(workout => workout.Users)
				.Include(workout => workout.Exercises)
				.ThenInclude(exercise => exercise.ExerciseInfo)
				.FirstOrDefaultAsync(workout => workout.Id == workoutIdTyped, cancellationToken)
				.ConfigureAwait(false);

			if (workout is null || !httpContext.User.CanModifyOrDelete(workout.Users, out _))
			{
				return TypedResults.NotFound("Workout not found.");
			}

			var exercise = workout.Exercises.FirstOrDefault(exercise => exercise.Index == exerciseIndex);
			if (exercise is null)
			{
				return TypedResults.NotFound("Exercise not found.");
			}

			if (!exercise.ExerciseInfo.AllowedMetricTypes.HasFlag(request.Metric.Type))
			{
				return TypedResults.BadRequest($"Invalid metric type: {request.Metric.Type}. Accepted types are: {exercise.ExerciseInfo.AllowedMetricTypes}.");
			}

			var set = new Workout.Exercise.Set(exercise, request.Index, request.Metric, request.Reps);

			exercise.Sets.Add(set);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			return TypedResults.Ok();
		});

		return builder;
	}
}
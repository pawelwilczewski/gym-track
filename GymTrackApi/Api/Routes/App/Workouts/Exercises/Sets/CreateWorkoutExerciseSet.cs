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

internal sealed class CreateWorkoutExerciseSet : IEndpoint
{
	public static async Task<Results<Created, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromRoute] int exerciseIndex,
		[FromBody] CreateWorkoutExerciseSetRequest request,
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

		// TODO Pawel: check if already exists at this index! Same for i.e. exercise, exerciseInfo step etc.! (add to tests)

		if (!exercise.ExerciseInfo.AllowedMetricTypes.HasFlag(request.Metric.Type))
		{
			return TypedResults.ValidationProblem(new Dictionary<string, string[]>
			{
				{ "Metric", ["Metric type not allowed."] }
			});
		}

		var index = exercise.Sets.Count > 0 ? exercise.Sets.Select(set => set.Index).Max() + 1 : 0;
		var set = new Workout.Exercise.Set(exercise, index, request.Metric, repsCount);

		exercise.Sets.Add(set);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.Created($"{httpContext.Request.Path}/{set.Index}");
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler);
		return builder;
	}
}
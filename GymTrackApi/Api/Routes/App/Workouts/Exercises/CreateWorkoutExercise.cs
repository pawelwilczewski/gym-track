using Api.Common;
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Index = Domain.Models.Index;

namespace Api.Routes.App.Workouts.Exercises;

internal sealed class CreateWorkoutExercise : IEndpoint
{
	public static async Task<Results<Created, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid workoutId,
		[FromBody] CreateWorkoutExerciseRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		if (!Index.TryCreate(request.Index, out var index)) return ValidationErrors.NegativeIndex();

		var workoutIdTyped = new Id<Workout>(workoutId);
		var workout = await dataContext.Workouts.Include(workout => workout.Users)
			.FirstOrDefaultAsync(workout => workout.Id == workoutIdTyped, cancellationToken)
			.ConfigureAwait(false);

		if (workout is null) return TypedResults.NotFound("Workout not found.");
		if (!httpContext.User.CanModifyOrDelete(workout.Users)) return TypedResults.Forbid();

		var exerciseInfoId = new Id<ExerciseInfo>(request.ExerciseInfoId);
		var exerciseInfo = await dataContext.ExerciseInfos.AsNoTracking()
			.Include(exerciseInfo => exerciseInfo.Users)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == exerciseInfoId, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanAccess(exerciseInfo.Users)) return TypedResults.Forbid();

		var exercise = new Workout.Exercise(workoutIdTyped, index, exerciseInfoId);

		workout.Exercises.Add(exercise);
		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return TypedResults.Created();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("", Handler);
		return builder;
	}
}
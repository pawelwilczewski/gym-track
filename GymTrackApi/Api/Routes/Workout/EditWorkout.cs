using System.Diagnostics;
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Workout;

internal sealed class EditWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("/{id:guid}", Delete)
			.RequireAuthorization();

		return builder;

		async Task<Results<Ok, NotFound, UnauthorizedHttpResult, BadRequest<string>>> Delete(
			HttpContext httpContext,
			Guid id,
			[FromBody] EditWorkoutRequest editWorkout,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken)
		{
			if (Name.TryCreate(editWorkout.Name, out var name) is TextValidationResult.Invalid invalid)
			{
				return TypedResults.BadRequest(invalid.Error);
			}

			var workoutId = new Id<Domain.Models.Workout.Workout>(id);
			var workout = await dataContext.Workouts
				.Include(workout => workout.UserWorkouts)
				.FirstOrDefaultAsync(
					workout => workout.Id == workoutId,
					cancellationToken)
				.ConfigureAwait(false);

			if (workout is null) return TypedResults.NotFound();

			switch (workout.CanDeleteOrModify(httpContext.User))
			{
				case Domain.Models.Workout.Workout.CanModifyResult.Yes:
				{
					workout.Name = name;
					await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
					return TypedResults.Ok();
				}
				case Domain.Models.Workout.Workout.CanModifyResult.Unauthorized:   return TypedResults.Unauthorized();
				case Domain.Models.Workout.Workout.CanModifyResult.NotFound:       return TypedResults.NotFound();
				case Domain.Models.Workout.Workout.CanModifyResult.ProhibitShared: return TypedResults.BadRequest("Can't delete shared workout");
				default:                                                           throw new UnreachableException();
			}
		}
	}
}
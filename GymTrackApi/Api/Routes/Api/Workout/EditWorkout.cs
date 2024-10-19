using Api.Common;
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Workout;
using Domain.Validation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Api.Workout;

internal sealed class EditWorkout : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("/{id:guid}", async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>> (
				HttpContext httpContext,
				Guid id,
				[FromBody] EditWorkoutRequest editWorkout,
				[FromServices] IDataContext dataContext,
				CancellationToken cancellationToken) =>
			{
				var workoutId = new Id<Domain.Models.Workout.Workout>(id);
				var workout = await dataContext.Workouts
					.Include(workout => workout.Users)
					.FirstOrDefaultAsync(
						workout => workout.Id == workoutId,
						cancellationToken)
					.ConfigureAwait(false);

				if (workout is null) return TypedResults.NotFound();

				return await httpContext.User.CanModifyOrDeleteWorkout(workout.Users)
					.ToResult(async () =>
					{
						if (workout.Name.Set(editWorkout.Name) is TextValidationResult.Invalid invalid)
						{
							return TypedResults.BadRequest(invalid.Error);
						}

						await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
						return TypedResults.Ok();
					});
			})
			.RequireAuthorization();

		return builder;
	}
}
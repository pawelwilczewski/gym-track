using Api.Common;
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Validation;
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

		async Task<IResult> Delete(
			HttpContext httpContext,
			Guid id,
			[FromBody] EditWorkoutRequest editWorkout,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken)
		{
			var workoutId = new Id<Domain.Models.Workout.Workout>(id);
			var workout = await dataContext.Workouts
				.Include(workout => workout.UserWorkouts)
				.FirstOrDefaultAsync(
					workout => workout.Id == workoutId,
					cancellationToken)
				.ConfigureAwait(false);

			if (workout is null) return TypedResults.NotFound();

			return await workout.CanDeleteOrModify(httpContext.User)
				.ToResult(async () =>
				{
					if (workout.Name.Set(editWorkout.Name) is TextValidationResult.Invalid invalid)
					{
						return TypedResults.BadRequest(invalid.Error);
					}

					await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
					return TypedResults.Ok();
				});
		}
	}
}
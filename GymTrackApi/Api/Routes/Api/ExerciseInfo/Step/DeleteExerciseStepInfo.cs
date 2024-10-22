using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Api.ExerciseInfo.Step;

internal sealed class DeleteExerciseStepInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("/{index:int}", async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromRoute] int index,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var id = new Id<Domain.Models.Workout.ExerciseInfo>(exerciseInfoId);
			var exerciseStepInfo = await dataContext.ExerciseStepInfos
				.Include(exerciseStepInfo => exerciseStepInfo.ExerciseInfo)
				.FirstOrDefaultAsync(exerciseStepInfo =>
					exerciseStepInfo.ExerciseInfoId == id
					&& exerciseStepInfo.Index == index, cancellationToken)
				.ConfigureAwait(false);

			if (exerciseStepInfo is null) return TypedResults.NotFound();

			var exerciseInfo = exerciseStepInfo.ExerciseInfo;
			if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users, out _))
			{
				return TypedResults.NotFound();
			}

			exerciseInfo.Steps.Remove(exerciseStepInfo);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			return TypedResults.Ok();
		});

		return builder;
	}
}
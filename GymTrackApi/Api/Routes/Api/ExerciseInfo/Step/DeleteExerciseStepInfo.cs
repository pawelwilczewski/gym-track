using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Api.ExerciseInfo.Step;

internal sealed class DeleteExerciseStepInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("/delete", async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>> (
			HttpContext httpContext,
			[FromBody] ExerciseStepInfoKey exerciseStepInfoKey,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var exerciseInfoId = new Id<Domain.Models.Workout.ExerciseInfo>(exerciseStepInfoKey.ExerciseInfoId);
			var exerciseStepInfo = await dataContext.ExerciseStepInfos
				.Include(exerciseStepInfo => exerciseStepInfo.ExerciseInfo)
				.FirstOrDefaultAsync(exerciseStepInfo =>
					exerciseStepInfo.ExerciseInfoId == exerciseInfoId
					&& exerciseStepInfo.Index == exerciseStepInfoKey.Index, cancellationToken)
				.ConfigureAwait(false);

			if (exerciseStepInfo is null) return TypedResults.NotFound();

			var exerciseInfo = exerciseStepInfo.ExerciseInfo;
			if (httpContext.User.CanModifyOrDelete(exerciseInfo.Users) is not CanModifyResult.Ok)
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
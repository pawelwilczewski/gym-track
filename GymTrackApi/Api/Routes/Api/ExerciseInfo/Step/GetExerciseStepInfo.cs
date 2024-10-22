using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Api.ExerciseInfo.Step;

internal sealed class GetExerciseStepInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("/{index:int}", async Task<Results<Ok<GetExerciseStepInfoResponse>, NotFound>> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseId,
			[FromRoute] int index,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var exerciseInfoId = new Id<Domain.Models.Workout.ExerciseInfo>(exerciseId);
			var exerciseInfo = await dataContext.ExerciseInfos
				.Include(exerciseInfo => exerciseInfo.Users)
				.Include(exerciseInfo => exerciseInfo.Steps)
				.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == exerciseInfoId, cancellationToken);

			if (exerciseInfo is null || !httpContext.User.CanAccess(exerciseInfo.Users))
			{
				return TypedResults.NotFound();
			}

			var stepInfo = exerciseInfo.Steps.FirstOrDefault(exerciseStepInfo => exerciseStepInfo.Index == index);
			if (stepInfo is null) return TypedResults.NotFound();

			return TypedResults.Ok(new GetExerciseStepInfoResponse(
				stepInfo.Index,
				stepInfo.Description.ToString(),
				stepInfo.ImageFile.Reduce(null)?.ToString()));
		});

		return builder;
	}
}
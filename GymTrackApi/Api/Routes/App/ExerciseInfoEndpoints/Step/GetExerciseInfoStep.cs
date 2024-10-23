using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfoEndpoints.Step;

internal sealed class GetExerciseInfoStep : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("/{index:int}", async Task<Results<Ok<GetExerciseInfoStepResponse>, NotFound>> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromRoute] int index,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var id = new Id<ExerciseInfo>(exerciseInfoId);
			var exerciseInfo = await dataContext.ExerciseInfos
				.AsNoTracking()
				.Include(exerciseInfo => exerciseInfo.Users)
				.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == index))
				.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == id, cancellationToken);

			if (exerciseInfo is null || !httpContext.User.CanAccess(exerciseInfo.Users))
			{
				return TypedResults.NotFound();
			}

			var stepInfo = exerciseInfo.Steps.SingleOrDefault();
			if (stepInfo is null) return TypedResults.NotFound();

			return TypedResults.Ok(new GetExerciseInfoStepResponse(
				stepInfo.Index,
				stepInfo.Description.ToString(),
				stepInfo.ImageFile.Reduce(null)?.ToString()));
		});

		return builder;
	}
}
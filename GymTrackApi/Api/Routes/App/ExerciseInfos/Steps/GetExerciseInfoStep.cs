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

namespace Api.Routes.App.ExerciseInfos.Steps;

internal sealed class GetExerciseInfoStep : IEndpoint
{
	public static async Task<Results<Ok<GetExerciseInfoStepResponse>, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromRoute] int index,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		if (!Index.TryCreate(index, out var indexTyped)) return ValidationErrors.NegativeIndex();

		var id = new Id<ExerciseInfo>(exerciseInfoId);
		var exerciseInfo = await dataContext.ExerciseInfos.AsNoTracking()
			.Include(exerciseInfo => exerciseInfo.Users)
			.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == indexTyped))
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == id, cancellationToken);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanAccess(exerciseInfo.Users)) return TypedResults.Forbid();

		var step = exerciseInfo.Steps.SingleOrDefault();
		if (step is null) return TypedResults.NotFound("Step not found.");

		return TypedResults.Ok(new GetExerciseInfoStepResponse(step.Index.IntValue, step.Description.ToString(), step.ImageFile?.ToString()));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{index:int}", Handler);
		return builder;
	}
}
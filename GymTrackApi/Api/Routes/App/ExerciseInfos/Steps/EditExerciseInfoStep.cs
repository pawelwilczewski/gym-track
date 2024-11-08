using Api.Common;
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos.Steps;

internal sealed class EditExerciseInfoStep : IEndpoint
{
	public static async Task<Results<NoContent, NotFound<string>, ForbidHttpResult, ValidationProblem>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromRoute] int index,
		[FromBody] EditExerciseInfoStepRequest request,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		if (index < 0) return ValidationErrors.NegativeIndex();

		var id = new Id<ExerciseInfo>(exerciseInfoId);
		var exerciseInfo = await dataContext.ExerciseInfos
			.Include(exerciseInfo => exerciseInfo.Users)
			.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == index))
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == id, cancellationToken)
			.ConfigureAwait(false);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users)) return TypedResults.Forbid();

		var step = exerciseInfo.Steps.SingleOrDefault();
		if (step is null) return TypedResults.NotFound("Step not found.");

		if (!step.Description.TrySet(request.Description, out var error))
		{
			return error.ToValidationProblem("Description");
		}

		await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		return TypedResults.NoContent();
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("{index:int}", Handler);
		return builder;
	}
}
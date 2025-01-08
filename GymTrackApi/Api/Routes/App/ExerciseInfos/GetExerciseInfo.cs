using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Common;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos;

internal sealed class GetExerciseInfo : IEndpoint
{
	public static async Task<Results<Ok<GetExerciseInfoResponse>, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var typedExerciseInfoId = new Id<ExerciseInfo>(exerciseInfoId);
		var exerciseInfo = await dataContext.ExerciseInfos.AsNoTracking()
			.Include(exerciseInfo => exerciseInfo.Users)
			.Include(exerciseInfo => exerciseInfo.Exercises)
			.Include(exerciseInfo => exerciseInfo.Steps)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == typedExerciseInfoId, cancellationToken);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanAccess(exerciseInfo.Users)) return TypedResults.Forbid();

		return TypedResults.Ok(new GetExerciseInfoResponse(
			exerciseInfo.Id.Value,
			exerciseInfo.Name.ToString(),
			exerciseInfo.Description.ToString(),
			exerciseInfo.AllowedMetricTypes,
			exerciseInfo.ThumbnailImage?.ToString(),
			exerciseInfo.Steps
				.Select(step => new ExerciseInfoStepKey(typedExerciseInfoId.Value, step.Index))
				.ToList()));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{exerciseInfoId:guid}", Handler);
		return builder;
	}
}
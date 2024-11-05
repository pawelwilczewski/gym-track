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
		[FromRoute] Guid id,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var exerciseInfoId = new Id<ExerciseInfo>(id);
		var exerciseInfo = await dataContext.ExerciseInfos.AsNoTracking()
			.Include(exerciseInfo => exerciseInfo.Users)
			.Include(exerciseInfo => exerciseInfo.Exercises)
			.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == exerciseInfoId, cancellationToken);

		if (exerciseInfo is null) return TypedResults.NotFound("Exercise info not found.");
		if (!httpContext.User.CanAccess(exerciseInfo.Users)) return TypedResults.Forbid();

		return TypedResults.Ok(new GetExerciseInfoResponse(
			exerciseInfo.Name.ToString(),
			exerciseInfo.Description.ToString(),
			exerciseInfo.AllowedMetricTypes,
			exerciseInfo.ThumbnailImage.ToString(),
			exerciseInfo.Exercises.Select(exercise => new WorkoutExerciseKey(exercise.WorkoutId.Value, exercise.Index.IntValue))
				.ToList()));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{id:guid}", Handler);
		return builder;
	}
}
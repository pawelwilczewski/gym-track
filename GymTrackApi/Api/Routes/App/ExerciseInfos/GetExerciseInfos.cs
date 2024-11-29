using Api.Dtos;
using Application.Persistence;
using Domain.Common;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos;

internal sealed class GetExerciseInfos : IEndpoint
{
	public static async Task<Results<Ok<List<GetExerciseInfoResponse>>, NotFound<string>, ForbidHttpResult>> Handler(
		HttpContext httpContext,
		[FromServices] IDataContext dataContext,
		CancellationToken cancellationToken)
	{
		var userId = httpContext.User.GetUserId();
		var isAdmin = httpContext.User.IsInRole(Role.ADMINISTRATOR);
		var exerciseInfos = dataContext.ExerciseInfos
			.Where(exerciseInfo => exerciseInfo.Users.Count <= 0 || isAdmin || exerciseInfo.Users.Any(user => user.UserId == userId))
			.AsNoTracking();
		var exerciseInfosResponse = exerciseInfos.Select(exerciseInfo => new GetExerciseInfoResponse(
			exerciseInfo.Id.Value,
			exerciseInfo.Name.ToString(),
			exerciseInfo.Description.ToString(),
			exerciseInfo.AllowedMetricTypes,
			exerciseInfo.ThumbnailImage != null ? exerciseInfo.ThumbnailImage.ToString() : null,
			exerciseInfo.Steps.Select(step => new ExerciseInfoStepKey(exerciseInfo.Id.Value, step.Index.IntValue)).ToList()));
		return TypedResults.Ok(await exerciseInfosResponse
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false));
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("", Handler);
		return builder;
	}
}
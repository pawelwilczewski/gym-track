using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Api.ExerciseInfo;

internal sealed class GetExerciseInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("/{id:guid}", async Task<Results<Ok<GetExerciseInfoResponse>, NotFound>> (
			HttpContext httpContext,
			[FromRoute] Guid id,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var exerciseInfoId = new Id<Domain.Models.Workout.ExerciseInfo>(id);
			var exerciseInfo = await dataContext.ExerciseInfos
				.Include(exerciseInfo => exerciseInfo.Users)
				.Include(exerciseInfo => exerciseInfo.Exercises)
				.FirstOrDefaultAsync(exerciseInfo => exerciseInfo.Id == exerciseInfoId, cancellationToken);

			if (exerciseInfo is null || !httpContext.User.CanAccess(exerciseInfo.Users)) return TypedResults.NotFound();

			return TypedResults.Ok(new GetExerciseInfoResponse(
				exerciseInfo.Name.ToString(),
				exerciseInfo.Description.ToString(),
				exerciseInfo.AllowedMetricTypes,
				exerciseInfo.ThumbnailImage.ToString(),
				exerciseInfo.Exercises.Select(exercise => new ExerciseKey(exercise.WorkoutId.Value, exercise.Index)).ToList()));
		});

		return builder;
	}
}
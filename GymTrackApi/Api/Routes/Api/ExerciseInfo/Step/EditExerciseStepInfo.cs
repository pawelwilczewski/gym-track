using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Api.ExerciseInfo.Step;

internal sealed class EditExerciseStepInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("/{index:int}", async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseId,
			[FromRoute] int index,
			[FromBody] EditExerciseStepInfoRequest request,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var exerciseInfoId = new Id<Domain.Models.Workout.ExerciseInfo>(exerciseId);
			var exerciseInfo = await dataContext.ExerciseInfos
				.Include(exerciseInfo => exerciseInfo.Users)
				.Include(exerciseInfo => exerciseInfo.Steps)
				.FirstOrDefaultAsync(
					exerciseInfo => exerciseInfo.Id == exerciseInfoId,
					cancellationToken)
				.ConfigureAwait(false);

			if (exerciseInfo is null || !httpContext.User.CanModifyOrDelete(exerciseInfo.Users, out _))
			{
				return TypedResults.NotFound();
			}

			var step = exerciseInfo.Steps.FirstOrDefault(step => step.Index == index);
			if (step is null) return TypedResults.NotFound();

			if (!step.Description.TrySet(request.Description, out var invalid))
			{
				return TypedResults.BadRequest(invalid.Error);
			}

			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			return TypedResults.Ok();
		});

		return builder;
	}
}
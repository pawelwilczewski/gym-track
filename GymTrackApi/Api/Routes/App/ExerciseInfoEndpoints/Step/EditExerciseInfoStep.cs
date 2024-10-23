using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfoEndpoints.Step;

internal sealed class EditExerciseInfoStep : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("/{index:int}", async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromRoute] int index,
			[FromBody] EditExerciseInfoStepRequest request,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var id = new Id<ExerciseInfo>(exerciseInfoId);
			var exerciseInfo = await dataContext.ExerciseInfos
				.Include(exerciseInfo => exerciseInfo.Users)
				.Include(exerciseInfo => exerciseInfo.Steps.Where(step => step.Index == index))
				.FirstOrDefaultAsync(
					exerciseInfo => exerciseInfo.Id == id,
					cancellationToken)
				.ConfigureAwait(false);

			if (exerciseInfo is null || !httpContext.User.CanModifyOrDelete(exerciseInfo.Users, out _))
			{
				return TypedResults.NotFound();
			}

			var step = exerciseInfo.Steps.SingleOrDefault();
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
using Api.Common;
using Api.Dtos;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.Api.ExerciseInfo;

internal sealed class EditExerciseInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("/{id:guid}", async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>> (
			HttpContext httpContext,
			Guid id,
			[FromBody] EditExerciseInfoRequest request,
			[FromServices] IDataContext dataContext,
			CancellationToken cancellationToken) =>
		{
			var exerciseInfoId = new Id<Domain.Models.Workout.ExerciseInfo>(id);
			var exerciseInfo = await dataContext.ExerciseInfos
				.Include(exerciseInfo => exerciseInfo.Users)
				.FirstOrDefaultAsync(
					exerciseInfo => exerciseInfo.Id == exerciseInfoId,
					cancellationToken)
				.ConfigureAwait(false);

			if (exerciseInfo is null) return TypedResults.NotFound();

			return await httpContext.User.CanModifyOrDelete(exerciseInfo.Users)
				.ToResult(async () =>
				{
					if (!exerciseInfo.Name.TrySet(request.Name, out var invalidName))
					{
						return TypedResults.BadRequest(invalidName.Error);
					}

					if (!exerciseInfo.Description.TrySet(request.Description, out var invalidDescription))
					{
						return TypedResults.BadRequest(invalidDescription.Error);
					}

					exerciseInfo.AllowedMetricTypes = request.AllowedMetricTypes;

					await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
					return TypedResults.Ok();
				});
		});

		return builder;
	}
}
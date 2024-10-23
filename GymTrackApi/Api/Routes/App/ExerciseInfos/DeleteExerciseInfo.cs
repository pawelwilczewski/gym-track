using Api.Common;
using Api.Files;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos;

internal sealed class DeleteExerciseInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("/{id:guid}", async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>> (
			HttpContext httpContext,
			Guid id,
			[FromServices] IDataContext dataContext,
			IWebHostEnvironment environment,
			CancellationToken cancellationToken) =>
		{
			var exerciseInfoId = new Id<ExerciseInfo>(id);
			var exerciseInfo = await dataContext.ExerciseInfos
				.Include(exerciseInfo => exerciseInfo.Users)
				.FirstOrDefaultAsync(
					exerciseInfo => exerciseInfo.Id == exerciseInfoId,
					cancellationToken)
				.ConfigureAwait(false);

			if (exerciseInfo is null) return TypedResults.NotFound();
			if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users, out var reason)) return reason.ToResult();

			File.Delete(Paths.UrlToLocal(exerciseInfo.ThumbnailImage.ToString(), environment));

			dataContext.ExerciseInfos.Remove(exerciseInfo);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			return TypedResults.Ok();
		});

		return builder;
	}
}
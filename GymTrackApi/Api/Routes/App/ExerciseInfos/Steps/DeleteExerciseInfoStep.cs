using Api.Files;
using Application.Persistence;
using Domain.Models;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Routes.App.ExerciseInfos.Steps;

internal sealed class DeleteExerciseInfoStep : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("/{index:int}", async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult>> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromRoute] int index,
			[FromServices] IDataContext dataContext,
			IWebHostEnvironment environment,
			CancellationToken cancellationToken) =>
		{
			var id = new Id<ExerciseInfo>(exerciseInfoId);
			var exerciseInfoStep = await dataContext.ExerciseInfoSteps
				.Include(exerciseInfoStep => exerciseInfoStep.ExerciseInfo)
				.ThenInclude(exerciseInfo => exerciseInfo.Users)
				.FirstOrDefaultAsync(exerciseInfoStep =>
					exerciseInfoStep.ExerciseInfoId == id
					&& exerciseInfoStep.Index == index, cancellationToken)
				.ConfigureAwait(false);

			if (exerciseInfoStep is null) return TypedResults.NotFound();

			var exerciseInfo = exerciseInfoStep.ExerciseInfo;
			if (!httpContext.User.CanModifyOrDelete(exerciseInfo.Users, out _))
			{
				return TypedResults.NotFound();
			}

			if (exerciseInfoStep.ImageFile.Reduce(null) is not null)
			{
				File.Delete(Paths.UrlToLocal(exerciseInfoStep.ImageFile.Reduce(null!).ToString(), environment));
			}

			exerciseInfo.Steps.Remove(exerciseInfoStep);
			await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

			return TypedResults.Ok();
		});

		return builder;
	}
}
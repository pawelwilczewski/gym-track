using Application.ExerciseInfo.Step.Commands;
using Domain.Common;
using Domain.Models;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfos.Steps;

using ResultType = Results<NoContent, NotFound>;

internal sealed class DeleteExerciseInfoStep : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromRoute] int stepIndex,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(new DeleteExerciseInfoStepCommand(
				new Id<ExerciseInfo>(exerciseInfoId),
				stepIndex,
				httpContext.User.GetUserId()), cancellationToken)
			.ConfigureAwait(false);

		return result.Match<ResultType>(
			success => TypedResults.NoContent(),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{stepIndex:int}", Handler);
		return builder;
	}
}
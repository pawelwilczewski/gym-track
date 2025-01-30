using Application.ExerciseInfo.Step.Dtos;
using Application.ExerciseInfo.Step.Queries;
using Domain.Common;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfos.Steps;

using ResultType = Results<Ok<GetExerciseInfoStepResponse>, NotFound>;

internal sealed class GetExerciseInfoStep : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{stepIndex:int}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromRoute] int stepIndex,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var response = await sender.Send(
					new GetExerciseInfoStepQuery(
						ExerciseInfoId.From(exerciseInfoId),
						ExerciseInfoStepIndex.From(stepIndex),
						httpContext.User.GetUserId()),
					cancellationToken)
				.ConfigureAwait(false);

			return response.Match<ResultType>(
				success => TypedResults.Ok(success.Value),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
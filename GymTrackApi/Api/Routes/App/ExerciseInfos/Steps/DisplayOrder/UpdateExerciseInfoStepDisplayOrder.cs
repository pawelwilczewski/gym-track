using Api.Dtos;
using Application.ExerciseInfo.Step.DisplayOrder.Commands;
using Domain.Common;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfos.Steps.DisplayOrder;

using ResultType = Results<NoContent, NotFound>;

internal sealed class UpdateExerciseInfoStepDisplayOrder : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPut("", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromRoute] int stepIndex,
			[FromBody] UpdateDisplayOrderRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(new UpdateExerciseInfoStepDisplayOrderCommand(
					ExerciseInfoId.From(exerciseInfoId),
					stepIndex,
					request.DisplayOrder,
					httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
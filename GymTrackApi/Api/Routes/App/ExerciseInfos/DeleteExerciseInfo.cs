using Application.ExerciseInfo.Commands;
using Domain.Common;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfos;

using ResultType = Results<NoContent, NotFound>;

internal sealed class DeleteExerciseInfo : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapDelete("{exerciseInfoId:guid}", async Task<ResultType> (
			HttpContext httpContext,
			[FromRoute] Guid exerciseInfoId,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var result = await sender.Send(
					new DeleteExerciseInfoCommand(
						ExerciseInfoId.From(exerciseInfoId),
						httpContext.User.GetUserId()), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				notFound => TypedResults.NotFound());
		});

		return builder;
	}
}
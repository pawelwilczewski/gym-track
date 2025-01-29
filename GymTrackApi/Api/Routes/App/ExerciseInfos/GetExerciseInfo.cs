using Application.ExerciseInfo.Dtos;
using Application.ExerciseInfo.Queries;
using Domain.Common;
using Domain.Models.ExerciseInfo;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfos;

using ResultType = Results<Ok<GetExerciseInfoResponse>, NotFound>;

internal sealed class GetExerciseInfo : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromRoute] Guid exerciseInfoId,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var response = await sender.Send(
				new GetExerciseInfoQuery(ExerciseInfoId.From(exerciseInfoId), httpContext.User.GetUserId()), cancellationToken)
			.ConfigureAwait(false);

		return response.Match<ResultType>(
			success => TypedResults.Ok(success.Value),
			notFound => TypedResults.NotFound());
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("{exerciseInfoId:guid}", Handler);
		return builder;
	}
}
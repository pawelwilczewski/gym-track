using Application.ExerciseInfo.Dtos;
using Application.ExerciseInfo.Queries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.App.ExerciseInfos;

using ResultType = Ok<List<GetExerciseInfoResponse>>;

internal sealed class GetExerciseInfos : IEndpoint
{
	public static async Task<ResultType> Handler(
		HttpContext httpContext,
		[FromServices] ISender sender,
		CancellationToken cancellationToken)
	{
		var result = await sender.Send(
				new GetExerciseInfosQuery(httpContext.User.GetUserId()),
				cancellationToken)
			.ConfigureAwait(false);

		return TypedResults.Ok(result.Value);
	}

	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("", Handler);
		return builder;
	}
}
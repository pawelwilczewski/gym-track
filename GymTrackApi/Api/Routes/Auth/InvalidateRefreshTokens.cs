using Application.Auth.Commands;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

using ResultType = NoContent;

internal sealed class InvalidateRefreshTokens : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/invalidate-refresh-tokens", async Task<ResultType> (
				HttpContext httpContext,
				[FromServices] ISender sender,
				CancellationToken cancellationToken) =>
			{
				var userId = httpContext.User.GetUserId();

				var result = await sender.Send(new InvalidateRefreshTokensCommand(
						userId), cancellationToken)
					.ConfigureAwait(false);

				return TypedResults.NoContent();
			})
			.RequireAuthorization();

		return builder;
	}
}
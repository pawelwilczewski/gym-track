using Api.Dtos;
using Application.Auth.Commands;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

using ResultType = Results<Ok<LoginResponse>, UnauthorizedHttpResult>;

internal sealed class RefreshAccess : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/refresh-access", async Task<ResultType> (
			HttpContext httpContext,
			[FromBody] RefreshAccessRequest request,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var tokenOrError = RefreshToken.TryFrom(request.RefreshToken);
			if (!tokenOrError.IsSuccess) return TypedResults.Unauthorized();

			var result = await sender.Send(new RefreshAccessCommand(
					tokenOrError.ValueObject,
					httpContext.Request.Headers.Origin!), cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.Ok(new LoginResponse(
					success.Value.AccessToken,
					success.Value.RefreshToken)),
				error => TypedResults.Unauthorized());
		});

		return builder;
	}
}
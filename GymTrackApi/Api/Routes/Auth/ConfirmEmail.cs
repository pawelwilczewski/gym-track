using Application.Auth.Commands;
using Domain.Common;
using Domain.Common.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

using ResultType = Results<NoContent, BadRequest>;

internal sealed class ConfirmEmail : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("/confirm-email", async Task<ResultType> (
			HttpContext httpContext,
			[FromQuery] string code,
			[FromServices] ISender sender,
			CancellationToken cancellationToken) =>
		{
			var codeOrError = EmailConfirmationCode.TryFrom(code);
			if (!codeOrError.IsSuccess)
			{
				return TypedResults.BadRequest();
			}

			var result = await sender.Send(new ConfirmEmailCommand(
						codeOrError.ValueObject, httpContext.User.GetUserId()),
					cancellationToken)
				.ConfigureAwait(false);

			return result.Match<ResultType>(
				success => TypedResults.NoContent(),
				error => TypedResults.BadRequest());
		});
		return builder;
	}
}
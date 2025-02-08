using Application.Auth.Commands;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

using ResultType = Results<NoContent, UnauthorizedHttpResult>;

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
				var result = await sender.Send(new ConfirmEmailCommand(
						code, httpContext.User.GetUserId()), cancellationToken)
					.ConfigureAwait(false);

				return result.Match<ResultType>(
					success => TypedResults.NoContent(),
					unauthorized => TypedResults.Unauthorized());
			})
			.Add(endpointBuilder =>
			{
				var finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
				AuthRoutes.ConfirmEmailEndpointName = $"confirm-email-{finalPattern}";
				endpointBuilder.Metadata.Add(new EndpointNameMetadata(AuthRoutes.ConfirmEmailEndpointName));
			});

		return builder;
	}
}
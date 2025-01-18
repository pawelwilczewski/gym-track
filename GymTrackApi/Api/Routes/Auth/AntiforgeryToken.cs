using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes.Auth;

internal sealed class AntiforgeryToken : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("antiforgery-token", Ok<string> (IAntiforgery antiforgery, HttpContext httpContext) =>
		{
			var tokens = antiforgery.GetAndStoreTokens(httpContext);
			return TypedResults.Ok(tokens.RequestToken);
		});

		return builder;
	}
}
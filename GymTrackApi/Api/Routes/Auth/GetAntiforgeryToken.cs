using Api.Dtos;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes.Auth;

internal sealed class GetAntiforgeryToken : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("antiforgery-token", Ok<GetAntiforgeryTokenResponse> (IAntiforgery antiforgery, HttpContext httpContext) =>
			{
				var tokens = antiforgery.GetAndStoreTokens(httpContext);
				if (tokens.RequestToken is null || tokens.HeaderName is null || tokens.CookieToken is null)
				{
					throw new Exception("Antiforgery token is not correctly configured");
				}

				return TypedResults.Ok(new GetAntiforgeryTokenResponse(
					tokens.HeaderName,
					tokens.FormFieldName,
					tokens.RequestToken));
			})
			.RequireAuthorization();

		return builder;
	}
}
using Domain.Models.Identity;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Routes.Identity;

internal sealed class Refresh : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/refresh",
			async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> (
				[FromBody] RefreshRequest refreshRequest,
				[FromServices] SignInManager<User> signInManager,
				[FromServices] IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
				[FromServices] TimeProvider timeProvider) =>
			{
				var refreshTokenProtector = bearerTokenOptions.Get(
						IdentityConstants.BearerScheme)
					.RefreshTokenProtector;
				var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

				// Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
				if (refreshTicket?.Properties.ExpiresUtc is not { } expiresUtc
					|| timeProvider.GetUtcNow() >= expiresUtc
					|| await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)
				{
					return TypedResults.Challenge();
				}

				var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
				return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
			});

		return builder;
	}
}
using Domain.Models.Identity;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Identity;

internal sealed class Login : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/login", async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> (
			[FromBody] LoginRequest login,
			[FromQuery] bool? useCookies,
			[FromQuery] bool? useSessionCookies,
			[FromServices] SignInManager<User> signInManager) =>
		{
			var useCookieScheme = useCookies == true || useSessionCookies == true;
			var isPersistent = useCookies == true && useSessionCookies != true;
			signInManager.AuthenticationScheme = useCookieScheme
				? IdentityConstants.ApplicationScheme
				: IdentityConstants.BearerScheme;

			var result = await signInManager.PasswordSignInAsync(
				login.Email, login.Password, isPersistent, true);

			if (result.RequiresTwoFactor)
			{
				if (!string.IsNullOrEmpty(login.TwoFactorCode))
				{
					result = await signInManager.TwoFactorAuthenticatorSignInAsync(
						login.TwoFactorCode, isPersistent, isPersistent);
				}
				else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
				{
					result = await signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
				}
			}

			if (!result.Succeeded)
			{
				return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
			}

			// The signInManager already produced the needed response in the form of a cookie or bearer token.
			return TypedResults.Empty;
		});

		return builder;
	}
}
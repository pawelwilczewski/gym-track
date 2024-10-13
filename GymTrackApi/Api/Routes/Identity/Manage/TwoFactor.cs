using System.Security.Claims;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Identity.Manage;

internal sealed class TwoFactor : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/2fa", async Task<Results<Ok<TwoFactorResponse>, ValidationProblem, NotFound>> (
			ClaimsPrincipal claimsPrincipal,
			[FromBody] TwoFactorRequest tfaRequest,
			[FromServices] SignInManager<User> signInManager) =>
		{
			var userManager = signInManager.UserManager;
			if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
			{
				return TypedResults.NotFound();
			}

			if (tfaRequest.Enable == true)
			{
				if (tfaRequest.ResetSharedKey)
				{
					return Identity.CreateValidationProblem("CannotResetSharedKeyAndEnable",
						"Resetting the 2fa shared key must disable 2fa until"
						+ " a 2fa token based on the new shared key is validated.");
				}

				if (string.IsNullOrEmpty(tfaRequest.TwoFactorCode))
				{
					return Identity.CreateValidationProblem("RequiresTwoFactor",
						"No 2fa token was provided by the request."
						+ " A valid 2fa token is required to enable 2fa.");
				}

				if (!await userManager.VerifyTwoFactorTokenAsync(
					user, userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.TwoFactorCode))
				{
					return Identity.CreateValidationProblem("InvalidTwoFactorCode",
						"The 2fa token provided by the request was invalid."
						+ " A valid 2fa token is required to enable 2fa.");
				}

				await userManager.SetTwoFactorEnabledAsync(user, true);
			}
			else if (tfaRequest.Enable == false || tfaRequest.ResetSharedKey)
			{
				await userManager.SetTwoFactorEnabledAsync(user, false);
			}

			if (tfaRequest.ResetSharedKey)
			{
				await userManager.ResetAuthenticatorKeyAsync(user);
			}

			string[]? recoveryCodes = null;
			if (tfaRequest.ResetRecoveryCodes
				|| (tfaRequest.Enable == true && await userManager.CountRecoveryCodesAsync(user) == 0))
			{
				var recoveryCodesEnumerable = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
				recoveryCodes = recoveryCodesEnumerable?.ToArray();
			}

			if (tfaRequest.ForgetMachine)
			{
				await signInManager.ForgetTwoFactorClientAsync();
			}

			var key = await userManager.GetAuthenticatorKeyAsync(user);
			if (string.IsNullOrEmpty(key))
			{
				await userManager.ResetAuthenticatorKeyAsync(user);
				key = await userManager.GetAuthenticatorKeyAsync(user);

				if (string.IsNullOrEmpty(key))
				{
					throw new NotSupportedException("The user manager must produce an authenticator key after reset.");
				}
			}

			return TypedResults.Ok(new TwoFactorResponse
			{
				SharedKey = key,
				RecoveryCodes = recoveryCodes,
				RecoveryCodesLeft = recoveryCodes?.Length ?? await userManager.CountRecoveryCodesAsync(user),
				IsTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user),
				IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user)
			});
		});

		return builder;
	}
}
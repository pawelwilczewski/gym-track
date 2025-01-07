using System.Security.Claims;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth.Manage;

internal sealed class Info : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("/info", async Task<Results<Ok<InfoResponse>, NotFound>> (
			ClaimsPrincipal claimsPrincipal,
			[FromServices] UserManager<User> userManager) =>
		{
			if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
			{
				return TypedResults.NotFound();
			}

			return TypedResults.Ok(await AuthRoutes.CreateInfoResponseAsync(user, userManager));
		});

		builder.MapPost("/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> (
			ClaimsPrincipal claimsPrincipal,
			[FromBody] InfoRequest infoRequest,
			HttpContext context,
			[FromServices] UserManager<User> userManager,
			[FromServices] IEmailSender<User> emailSender,
			[FromServices] LinkGenerator linkGenerator) =>
		{
			if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
			{
				return TypedResults.NotFound();
			}

			if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !AuthRoutes.IsEmailValid(infoRequest.NewEmail))
			{
				return AuthRoutes.CreateValidationProblem(IdentityResult.Failed(
					userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)));
			}

			if (!string.IsNullOrEmpty(infoRequest.NewPassword))
			{
				if (string.IsNullOrEmpty(infoRequest.OldPassword))
				{
					return AuthRoutes.CreateValidationProblem("OldPasswordRequired",
						"The old password is required to set a new password. "
						+ "If the old password is forgotten, use /reset-password.");
				}

				var changePasswordResult = await userManager.ChangePasswordAsync(
					user, infoRequest.OldPassword, infoRequest.NewPassword);
				if (!changePasswordResult.Succeeded)
				{
					return AuthRoutes.CreateValidationProblem(changePasswordResult);
				}
			}

			if (!string.IsNullOrEmpty(infoRequest.NewEmail))
			{
				var email = await userManager.GetEmailAsync(user);

				if (email != infoRequest.NewEmail)
				{
					await AuthRoutes.SendConfirmationEmailAsync(
						emailSender, user, userManager, context,
						linkGenerator, infoRequest.NewEmail, true);
				}
			}

			return TypedResults.Ok(await AuthRoutes.CreateInfoResponseAsync(user, userManager));
		});

		return builder;
	}
}
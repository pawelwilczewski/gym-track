using System.Security.Claims;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Identity.Manage;

internal sealed class Info : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> (
			ClaimsPrincipal claimsPrincipal,
			[FromServices] UserManager<User> userManager) =>
		{
			if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
			{
				return TypedResults.NotFound();
			}

			return TypedResults.Ok(await IdentityRoutes.CreateInfoResponseAsync(user, userManager));
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

			if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !IdentityRoutes.IsEmailValid(infoRequest.NewEmail))
			{
				return IdentityRoutes.CreateValidationProblem(IdentityResult.Failed(
					userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)));
			}

			if (!string.IsNullOrEmpty(infoRequest.NewPassword))
			{
				if (string.IsNullOrEmpty(infoRequest.OldPassword))
				{
					return IdentityRoutes.CreateValidationProblem("OldPasswordRequired",
						"The old password is required to set a new password. "
						+ "If the old password is forgotten, use /resetPassword.");
				}

				var changePasswordResult = await userManager.ChangePasswordAsync(
					user, infoRequest.OldPassword, infoRequest.NewPassword);
				if (!changePasswordResult.Succeeded)
				{
					return IdentityRoutes.CreateValidationProblem(changePasswordResult);
				}
			}

			if (!string.IsNullOrEmpty(infoRequest.NewEmail))
			{
				var email = await userManager.GetEmailAsync(user);

				if (email != infoRequest.NewEmail)
				{
					await IdentityRoutes.SendConfirmationEmailAsync(
						emailSender, user, userManager, context,
						linkGenerator, infoRequest.NewEmail, true);
				}
			}

			return TypedResults.Ok(await IdentityRoutes.CreateInfoResponseAsync(user, userManager));
		});

		return builder;
	}
}
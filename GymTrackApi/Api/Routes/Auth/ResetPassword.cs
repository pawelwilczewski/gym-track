using System.Text;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Api.Routes.Auth;

internal sealed class ResetPassword : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/reset-password", async Task<Results<Ok, ValidationProblem>> (
			[FromBody] ResetPasswordRequest resetRequest,
			[FromServices] UserManager<User> userManager) =>
		{
			var user = await userManager.FindByEmailAsync(resetRequest.Email);

			if (user is null || !await userManager.IsEmailConfirmedAsync(user))
			{
				// Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we
				// returned a 400 for an invalid code given a valid user email.
				return AuthRoutes.CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
			}

			IdentityResult result;
			try
			{
				var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));
				result = await userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);
			}
			catch (FormatException)
			{
				result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
			}

			if (!result.Succeeded)
			{
				return AuthRoutes.CreateValidationProblem(result);
			}

			return TypedResults.Ok();
		});

		return builder;
	}
}
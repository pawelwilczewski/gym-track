using System.Text;
using System.Text.Encodings.Web;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Api.Routes.Auth;

internal sealed class ForgotPassword : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/forgot-password", async Task<Results<Ok, ValidationProblem>> (
			[FromBody] ForgotPasswordRequest resetRequest,
			[FromServices] UserManager<User> userManager,
			[FromServices] IEmailSender<User> emailSender) =>
		{
			var user = await userManager.FindByEmailAsync(resetRequest.Email);

			if (user is not null && await userManager.IsEmailConfirmedAsync(user))
			{
				var code = await userManager.GeneratePasswordResetTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

				await emailSender.SendPasswordResetCodeAsync(
					user, resetRequest.Email, HtmlEncoder.Default.Encode(code));
			}

			// Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we
			// returned a 400 for an invalid code given a valid user email.
			return TypedResults.Ok();
		});

		return builder;
	}
}
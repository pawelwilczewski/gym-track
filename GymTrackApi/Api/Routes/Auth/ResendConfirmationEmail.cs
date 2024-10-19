using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

internal sealed class ResendConfirmationEmail : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/resendConfirmationEmail", async Task<Ok> (
			[FromBody] ResendConfirmationEmailRequest resendRequest,
			HttpContext context,
			[FromServices] UserManager<User> userManager,
			[FromServices] IEmailSender<User> emailSender,
			[FromServices] LinkGenerator linkGenerator) =>
		{
			if (await userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
			{
				return TypedResults.Ok();
			}

			await AuthRoutes.SendConfirmationEmailAsync(emailSender, user, userManager, context, linkGenerator, resendRequest.Email);
			return TypedResults.Ok();
		});

		return builder;
	}
}
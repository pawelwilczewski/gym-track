using Domain.Models.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Auth;

internal sealed class ResendConfirmationEmail : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/resend-confirmation-email", async Task<Ok> (
			[FromBody] ResendConfirmationEmailRequest resendRequest,
			HttpContext context,
			[FromServices] UserManager<User> userManager,
			[FromServices] IEmailSender<User> emailSender,
			[FromServices] LinkGenerator linkGenerator) =>
		{
			throw new NotImplementedException();

			return TypedResults.Ok();
		});

		return builder;
	}
}
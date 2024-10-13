using System.Text;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Api.Routes.Identity;

internal sealed class ConfirmEmail : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapGet("/confirmEmail", async Task<Results<ContentHttpResult, UnauthorizedHttpResult>> (
				[FromQuery] string userId,
				[FromQuery] string code,
				[FromQuery] string? changedEmail,
				[FromServices] UserManager<User> userManager) =>
			{
				if (await userManager.FindByIdAsync(userId) is not { } user)
				{
					// We could respond with a 404 instead of a 401 like Identity UI,
					// but that feels like unnecessary information.
					return TypedResults.Unauthorized();
				}

				try
				{
					code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
				}
				catch (FormatException)
				{
					return TypedResults.Unauthorized();
				}

				IdentityResult result;

				if (string.IsNullOrEmpty(changedEmail))
				{
					result = await userManager.ConfirmEmailAsync(user, code);
				}
				else
				{
					// As with Identity UI, email and username are one and the same. So when we update the email,
					// we need to update the username.
					result = await userManager.ChangeEmailAsync(user, changedEmail, code);

					if (result.Succeeded)
					{
						result = await userManager.SetUserNameAsync(user, changedEmail);
					}
				}

				if (!result.Succeeded)
				{
					return TypedResults.Unauthorized();
				}

				return TypedResults.Text("Thank you for confirming your email.");
			})
			.Add(endpointBuilder =>
			{
				var finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
				Identity.ConfirmEmailEndpointName = $"confirmEmail-{finalPattern}";
				endpointBuilder.Metadata.Add(new EndpointNameMetadata(Identity.ConfirmEmailEndpointName));
			});

		return builder;
	}
}
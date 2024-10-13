using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Identity;

internal sealed class Register : IEndpoint
{
	public IEndpointRouteBuilder Map(IEndpointRouteBuilder builder)
	{
		builder.MapPost("/register", async Task<Results<Ok, ValidationProblem>> (
			[FromBody] RegisterRequest registration,
			HttpContext context,
			[FromServices] UserManager<User> userManager,
			[FromServices] IUserStore<User> userStore,
			[FromServices] IEmailSender<User> emailSender,
			[FromServices] LinkGenerator linkGenerator) =>
		{
			if (!userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("`register` requires a user store with email support.");
			}

			var emailStore = (IUserEmailStore<User>)userStore;
			var email = registration.Email;

			if (string.IsNullOrEmpty(email) || !Identity.IsEmailValid(email))
			{
				return Identity.CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
			}

			var user = new User();
			await userStore.SetUserNameAsync(user, email, CancellationToken.None);
			await emailStore.SetEmailAsync(user, email, CancellationToken.None);
			var result = await userManager.CreateAsync(user, registration.Password);

			if (!result.Succeeded)
			{
				return Identity.CreateValidationProblem(result);
			}

			await Identity.SendConfirmationEmailAsync(emailSender, user, userManager, context, linkGenerator, email);
			return TypedResults.Ok();
		});

		return builder;
	}
}
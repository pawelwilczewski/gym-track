using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using Api.Routes.Auth.Manage;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;

namespace Api.Routes.Auth;

internal static class AuthRoutes
{
	internal static Func<object?, bool> IsEmailValid { get; } = new EmailAddressAttribute().IsValid;
	internal static string ConfirmEmailEndpointName { get; set; } = null!;

	public static IEndpointRouteBuilder MapAuthRoutes(this IEndpointRouteBuilder builder)
	{
		var root = builder.MapGroup("auth");
		root
			.WithTags("Auth")
			.Map(new Login())
			.Map(new Logout())
			.Map(new Register())
			.Map(new Refresh())
			.Map(new ConfirmEmail())
			.Map(new ResendConfirmationEmail())
			.Map(new ForgotPassword())
			.Map(new ResetPassword())
			.Map(new AntiforgeryToken())
			.MapManageRoutes();

		return builder;
	}

	internal static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription) =>
		TypedResults.ValidationProblem(new Dictionary<string, string[]>
		{
			{ errorCode, [errorDescription] }
		});

	internal static ValidationProblem CreateValidationProblem(IdentityResult result)
	{
		// We expect a single error code and description in the normal case.
		// This could be golfed with GroupBy and ToDictionary, but perf! :P
		Debug.Assert(!result.Succeeded);
		var errorDictionary = new Dictionary<string, string[]>(1);

		foreach (var error in result.Errors)
		{
			string[] newDescriptions;

			if (errorDictionary.TryGetValue(error.Code, out var descriptions))
			{
				newDescriptions = new string[descriptions.Length + 1];
				Array.Copy(descriptions, newDescriptions, descriptions.Length);
				newDescriptions[descriptions.Length] = error.Description;
			}
			else
			{
				newDescriptions = [error.Description];
			}

			errorDictionary[error.Code] = newDescriptions;
		}

		return TypedResults.ValidationProblem(errorDictionary);
	}

	internal static async Task<InfoResponse> CreateInfoResponseAsync(User user, UserManager<User> userManager) =>
		new()
		{
			Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
			IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user)
		};

	internal static async Task SendConfirmationEmailAsync(
		IEmailSender<User> emailSender,
		User user,
		UserManager<User> userManager,
		HttpContext context,
		LinkGenerator linkGenerator,
		string email,
		bool isChange = false)
	{
		if (ConfirmEmailEndpointName is null)
		{
			throw new NotSupportedException("No email confirmation endpoint was registered!");
		}

		var code = isChange
			? await userManager.GenerateChangeEmailTokenAsync(user, email)
			: await userManager.GenerateEmailConfirmationTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

		var userId = await userManager.GetUserIdAsync(user);
		var routeValues = new RouteValueDictionary
		{
			["userId"] = userId,
			["code"] = code
		};

		if (isChange)
		{
			// This is validated by the /confirmEmail endpoint on change.
			routeValues.Add("changedEmail", email);
		}

		var confirmEmailUrl = linkGenerator.GetUriByName(context, ConfirmEmailEndpointName, routeValues)
			?? throw new NotSupportedException($"Could not find endpoint named '{ConfirmEmailEndpointName}'.");

		await emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
	}
}
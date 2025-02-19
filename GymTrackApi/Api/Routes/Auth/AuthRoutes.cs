using System.ComponentModel.DataAnnotations;

namespace Api.Routes.Auth;

internal static class AuthRoutes
{
	internal static Func<object?, bool> IsEmailValid { get; } = new EmailAddressAttribute().IsValid;
	internal static string ConfirmEmailEndpointName { get; set; } = null!;

	public static IEndpointRouteBuilder MapAuthRoutes(this IEndpointRouteBuilder builder)
	{
		var auth = builder
			.MapGroup("auth")
			.WithTags("Auth")
			.Map(new Login())
			.Map(new Register())
			.Map(new RefreshAccess())
			.Map(new InvalidateRefreshTokens())
			.Map(new ConfirmEmail())
			.Map(new SendConfirmationEmail())
			.Map(new ForgotPassword())
			.Map(new ResetPassword())
			.Map(new GetAntiforgeryToken());

		return builder;
	}
}
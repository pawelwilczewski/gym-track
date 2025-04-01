namespace Api.Routes.Auth;

internal static class AuthRoutes
{
	public static IEndpointRouteBuilder MapAuthRoutes(this IEndpointRouteBuilder builder)
	{
		var auth = builder
			.MapGroup("auth")
			.WithTags("Auth")
			.Map(new Login())
			.Map(new Register())
			.Map(new RefreshLogin())
			.Map(new InvalidateRefreshTokens())
			.Map(new ConfirmEmail())
			.Map(new ResendConfirmationEmail())
			.Map(new ForgotPassword())
			.Map(new ResetPassword())
			.Map(new UpdatePassword())
			.Map(new GetAntiforgeryToken());

		return builder;
	}
}
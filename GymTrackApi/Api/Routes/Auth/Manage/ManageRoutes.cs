namespace Api.Routes.Auth.Manage;

internal static class ManageRoutes
{
	public static IEndpointRouteBuilder MapManageRoutes(this IEndpointRouteBuilder builder)
	{
		builder.MapGroup("manage")
			.RequireAuthorization()
			.Map(new Info())
			.Map(new TwoFactor());

		return builder;
	}
}
namespace Api.Routes;

internal static class Users
{
	public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder builder)
	{
		var group = builder.MapGroup("users");

		group.MapGet("{id:int}", (HttpContext httpContext, int id) =>
				Task.FromResult(Results.Ok(id)))
			.WithOpenApi();

		return builder;
	}
}
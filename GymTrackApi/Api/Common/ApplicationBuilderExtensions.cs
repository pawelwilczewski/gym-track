namespace Api.Common;

internal static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder Use404InsteadOf403(this IApplicationBuilder builder) =>
		builder.Use(async (context, next) =>
		{
			await next().ConfigureAwait(false);

			if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
			{
				context.Response.StatusCode = StatusCodes.Status404NotFound;
			}
		});

	public static IApplicationBuilder Strip404Body(this IApplicationBuilder builder) =>
		builder.Use(async (context, next) =>
		{
			if (context.Response.StatusCode == StatusCodes.Status404NotFound)
			{
				context.Response.Body = new MemoryStream();
			}

			await next().ConfigureAwait(false);
		});
}
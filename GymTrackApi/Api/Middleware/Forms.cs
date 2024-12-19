namespace Api.Middleware;

internal static class Forms
{
	public static IApplicationBuilder AddPutFormSupport(this IApplicationBuilder app)
	{
		app.Use(async (context, next) =>
		{
			if (context.Request.Method == "POST" && context.Request.Form.ContainsKey("_method"))
			{
				var method = context.Request.Form["_method"].ToString().ToUpper();
				if (method == "PUT")
				{
					context.Request.Method = "PUT";
				}
			}

			await next();
		});

		return app;
	}
}
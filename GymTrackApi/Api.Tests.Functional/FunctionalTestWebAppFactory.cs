using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Api.Tests.Functional;

public sealed class FunctionalTestWebAppFactory : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration((_, config) =>
		{
			config.Sources.Clear();

			config
				.AddJsonFile("appsettings.json", true)
				.AddJsonFile("appsettings.Test.json", false, true);
		});

		builder.UseEnvironment("Test");
	}
}
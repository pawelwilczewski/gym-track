using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using TUnit.Core.Interfaces;

namespace Api.Tests.Functional;

public sealed class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
	internal HttpClient Client { get; private set; } = default!;

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

	public Task InitializeAsync()
	{
		// don't remove - this makes sure we can create multiple clients later
		// (seems like because of concurrency we otherwise get an error that db doesn't exist in some tests!)
		Client = CreateClient();
		return Task.CompletedTask;
	}
}
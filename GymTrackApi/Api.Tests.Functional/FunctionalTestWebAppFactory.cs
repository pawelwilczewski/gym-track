using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Testcontainers.PostgreSql;
using TUnit.Core.Interfaces;

namespace Api.Tests.Functional;

public sealed class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
	private PostgreSqlContainer dbContainer = default!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureTestServices(services =>
		{
			services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
			services.RemoveAll(typeof(AppDbContext));
			services.AddDbContext<AppDbContext>(options => options
				.UseNpgsql(dbContainer.GetConnectionString())
				.EnableSensitiveDataLogging());
		});
	}

	public async Task InitializeAsync()
	{
		dbContainer = new PostgreSqlBuilder()
			.WithImage("postgres:latest")
			.WithDatabase("GymTrack-Test")
			.WithUsername("postgres")
			.WithPassword("postgres")
			.Build();

		await dbContainer.StartAsync().ConfigureAwait(false);
		await SetUpDb().ConfigureAwait(false);
		return;

		async Task SetUpDb()
		{
			await using var connection = new NpgsqlConnection(dbContainer.GetConnectionString());
			await connection.OpenAsync().ConfigureAwait(false);

			await using var setupCommand = new NpgsqlCommand(
				"CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";",
				connection);
			await setupCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

			await connection.CloseAsync().ConfigureAwait(false);
		}
	}

	public new async Task DisposeAsync()
	{
		await dbContainer.StopAsync().ConfigureAwait(false);
		await dbContainer.DisposeAsync().ConfigureAwait(false);

		await base.DisposeAsync().ConfigureAwait(false);
	}
}
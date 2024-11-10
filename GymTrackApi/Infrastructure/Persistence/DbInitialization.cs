using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Persistence;

public static class DbInitialization
{
	public static async Task InitializeDb(this IServiceProvider serviceProvider, IConfiguration configuration)
	{
		var dbSection = configuration.GetRequiredSection("Database");

		if (bool.TryParse(dbSection["DeleteExisting"], out var delete) && delete)
		{
			using var scope = serviceProvider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			await dbContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
		}

		if (bool.TryParse(dbSection["TryCreate"], out var create) && create)
		{
			await TryCreateDb(dbSection["ConnectionString"]!, async connection =>
				{
					await using var setupCommand = new NpgsqlCommand("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";", connection);
					await setupCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
				})
				.ConfigureAwait(false);
		}
	}

	private static async Task<bool> DbExists(string connectionString)
	{
		var createDbConnectionString = CreateConnectionStringWithoutDbName(connectionString, out var dbName);
		await using var connection = new NpgsqlConnection(createDbConnectionString);
		await connection.OpenAsync().ConfigureAwait(false);

		var checkDbCommandText = $"SELECT 1 FROM pg_database WHERE datname = '{dbName}';";
		await using var checkDbCommand = new NpgsqlCommand(checkDbCommandText, connection);
		return await checkDbCommand.ExecuteScalarAsync().ConfigureAwait(false) != null;
	}

	private static async Task<bool> TryCreateDb(string connectionString, Func<NpgsqlConnection, Task> createdDbSetup)
	{
		var createDbConnectionString = CreateConnectionStringWithoutDbName(connectionString, out var dbName);
		await using (var createDbConnection = new NpgsqlConnection(createDbConnectionString))
		{
			await createDbConnection.OpenAsync().ConfigureAwait(false);

			if (await DbExists(connectionString).ConfigureAwait(false)) return false;

			var createDbCommandText = $"CREATE DATABASE \"{dbName}\";";
			await using var createDbCommand = new NpgsqlCommand(createDbCommandText, createDbConnection);
			await createDbCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
		}

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync().ConfigureAwait(false);

			await createdDbSetup(connection);
		}

		return true;
	}

	private static string CreateConnectionStringWithoutDbName(string connectionString, out string? dbName)
	{
		var builder = new NpgsqlConnectionStringBuilder(connectionString);
		dbName = builder.Database;
		builder.Database = null;
		return builder.ConnectionString;
	}
}
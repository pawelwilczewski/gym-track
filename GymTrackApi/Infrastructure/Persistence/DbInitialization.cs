using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Persistence;

internal static class DbInitialization
{
	public static async Task InitializeDb(AppDbContext dbContext, IOptions<DatabaseSettings> databaseSettings)
	{
		var settings = databaseSettings.Value;

		if (settings.DeleteDbIfExists)
		{
			await dbContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
		}

		var created = false;
		if (settings.TryCreateDbIfNotExists)
		{
			created = await TryCreateDb(settings.ConnectionString, async connection =>
				{
					await using var setupCommand = new NpgsqlCommand(
						"CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";", connection);
					await setupCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
				})
				.ConfigureAwait(false);
		}

		// if just created db, we will always want to apply migrations (it's an empty db anyway)
		if (created || settings.AutoApplyMigrations)
		{
			await dbContext.Database.MigrateAsync().ConfigureAwait(false);
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

			await createdDbSetup(connection).ConfigureAwait(false);
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
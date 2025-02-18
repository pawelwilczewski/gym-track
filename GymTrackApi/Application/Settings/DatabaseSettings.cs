namespace Application.Settings;

public sealed record class DatabaseSettings(
	string ConnectionString,
	bool DeleteDbIfExists,
	bool TryCreateDbIfNotExists,
	bool AutoApplyMigrations,
	bool EnableSensitiveDataLogging)
{
	public DatabaseSettings() : this(
		null!,
		false,
		true,
		false,
		false) { }
}
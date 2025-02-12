namespace Infrastructure.Settings;

// TODO Pawel: consider strongly typing all of the settings properties!
internal sealed record class DatabaseSettings(
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
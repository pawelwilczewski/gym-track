namespace Infrastructure.Settings;

internal sealed record class EmailConfirmationSettings(
	int ExpiryTimeInMinutes)
{
	public EmailConfirmationSettings() : this(
		24 * 60) { }
}
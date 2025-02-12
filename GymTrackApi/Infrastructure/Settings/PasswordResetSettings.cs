namespace Infrastructure.Settings;

internal sealed record class PasswordResetSettings(
	int ExpiryTimeInMinutes)
{
	public PasswordResetSettings() : this(
		12 * 60) { }
}
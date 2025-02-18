namespace Application.Settings;

public sealed record class PasswordResetSettings(
	int ExpiryTimeInMinutes)
{
	public PasswordResetSettings() : this(
		12 * 60) { }
}
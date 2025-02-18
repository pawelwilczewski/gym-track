namespace Application.Settings;

public sealed record class EmailConfirmationSettings(
	int ExpiryTimeInMinutes)
{
	public EmailConfirmationSettings() : this(
		24 * 60) { }
}
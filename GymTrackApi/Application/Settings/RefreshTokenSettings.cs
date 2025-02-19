namespace Application.Settings;

public sealed record class RefreshTokenSettings(
	int ExpiryTimeInMinutes)
{
	public RefreshTokenSettings() : this(
		2 * 24 * 60) { }
}
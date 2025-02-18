namespace Application.Settings;

public sealed record class JwtSettings(
	string Issuer,
	string Key,
	double ExpirationInMinutes)
{
	public JwtSettings() : this(
		null!,
		null!,
		60) { }
}
namespace Application.Settings;

public sealed record class JwtSettings(
	string Issuer,
	string Key,
	double ExpirationInMinutes,
	bool ValidateAudience)
{
	public JwtSettings() : this(
		null!,
		null!,
		60,
		true) { }
}
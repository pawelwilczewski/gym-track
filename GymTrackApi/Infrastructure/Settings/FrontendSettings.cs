namespace Infrastructure.Settings;

internal sealed record class FrontendSettings(
	string BaseUrl,
	string ConfirmEmailRoute,
	string PasswordResetRoute)
{
	public FrontendSettings() : this(
		null!,
		null!,
		null!) { }

	public string BuildEmailConfirmationUrl(string code) =>
		$"{BaseUrl.TrimEnd('/')}/{ConfirmEmailRoute.Trim('/')}?code={code}";

	public string BuildPasswordResetUrl(string code) =>
		$"{BaseUrl.TrimEnd('/')}/{PasswordResetRoute.Trim('/')}?code={code}";
}
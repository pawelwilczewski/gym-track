namespace Infrastructure.Settings;

internal sealed record class FrontendSettings(
	string BaseUrl,
	string ConfirmEmailRoute)
{
	public FrontendSettings() : this(
		null!,
		null!) { }

	public string BuildEmailConfirmationUrl(string code) =>
		$"{BaseUrl.TrimEnd('/')}/{ConfirmEmailRoute.Trim('/')}?code={code}";
}
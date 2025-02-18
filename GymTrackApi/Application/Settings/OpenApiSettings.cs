namespace Application.Settings;

public sealed record class OpenApiSettings(
	bool Enabled,
	string Url)
{
	public OpenApiSettings() : this(
		false,
		null!) { }
}
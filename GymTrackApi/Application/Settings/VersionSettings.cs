namespace Application.Settings;

public sealed record class VersionSettings(
	int Major,
	int Minor)
{
	public VersionSettings() : this(1, 0) { }
}
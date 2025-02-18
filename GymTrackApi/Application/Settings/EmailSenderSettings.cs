namespace Application.Settings;

public sealed record class EmailSenderSettings(
	string ApiKey,
	EmailSenderSettings.SenderInfo Sender)
{
	public EmailSenderSettings() : this(
		null!,
		null!) { }

	public sealed record class SenderInfo(
		string Email,
		string Name)
	{
		public SenderInfo() : this(
			null!,
			null!) { }
	}
}
namespace Infrastructure.Settings;

internal sealed record class SendGridSettings(
	string ApiKey,
	SendGridSettings.SenderInfo Sender)
{
	public SendGridSettings() : this(
		null!,
		null!) { }

	internal sealed record class SenderInfo(
		string Email,
		string Name)
	{
		public SenderInfo() : this(
			null!,
			null!) { }
	}
}
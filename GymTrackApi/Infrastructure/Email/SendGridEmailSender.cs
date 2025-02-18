using Application.Email;
using Application.Settings;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Email;

internal sealed class SendGridEmailSender : IEmailSender
{
	private readonly SendGridClient client;
	private readonly EmailAddress from;

	public SendGridEmailSender(IOptions<EmailSenderSettings> sendGridSettings)
	{
		var settings = sendGridSettings.Value;
		client = new SendGridClient(settings.ApiKey);
		from = new EmailAddress(settings.Sender.Email, settings.Sender.Name);
	}

	public async Task SendEmail(Domain.Common.ValueObjects.EmailAddress address, string subject, string message, CancellationToken cancellationToken)
	{
		var email = MailHelper.CreateSingleEmail(
			from,
			new EmailAddress(address.Value),
			subject,
			message,
			message);

		if (email is null) throw new Exception("Couldn't create email to send.");

		await client.SendEmailAsync(email, cancellationToken).ConfigureAwait(false);
	}
}
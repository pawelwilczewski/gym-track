using Application.Email;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Email;

internal sealed class SendGridEmailSender : IEmailSender
{
	private readonly SendGridClient client;
	private readonly EmailAddress from;

	public SendGridEmailSender(IConfiguration configuration)
	{
		client = new SendGridClient(configuration["SendGrid:ApiKey"]);
		from = new EmailAddress(configuration["SendGrid:From:Email"], configuration["SendGrid:From:Name"]);
	}

	public async Task SendEmail(Domain.Common.ValueObjects.EmailAddress address, string subject, string message)
	{
		var email = MailHelper.CreateSingleEmail(
			from,
			new EmailAddress(address.Value),
			subject,
			message,
			message);

		if (email is null) throw new Exception("Couldn't create email to send.");

		await client.SendEmailAsync(email).ConfigureAwait(false);
	}
}
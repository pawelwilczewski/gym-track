using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Email;

internal sealed class SendGridIdentityEmailSender : IEmailSender<User>
{
	private readonly SendGridClient client;
	private readonly EmailAddress from;

	public SendGridIdentityEmailSender(IConfiguration configuration)
	{
		client = new SendGridClient(configuration["SendGrid:ApiKey"]);
		from = new EmailAddress(configuration["SendGrid:From:Email"], configuration["SendGrid:From:Name"]);
	}

	public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink) =>
		SendEmailAsync(user, "Account Confirmation", $"Confirm your email by going to: {confirmationLink}");

	public Task SendPasswordResetLinkAsync(User user, string email, string resetLink) =>
		SendEmailAsync(user, "Password Reset Request", $"Reset your password by going to: {resetLink}");

	public Task SendPasswordResetCodeAsync(User user, string email, string resetCode) =>
		SendEmailAsync(user, "Password Reset Request", $"Use this code to reset your password: {resetCode}");

	private async Task SendEmailAsync(User to, string subject, string content)
	{
		var message = MailHelper.CreateSingleEmail(from, new EmailAddress(to.Email), subject, content, content);
		if (message is null) throw new Exception("Couldn't create email to send.");

		await client.SendEmailAsync(message).ConfigureAwait(false);
	}
}
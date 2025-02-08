using Domain.Common.ValueObjects;

namespace Application.Email;

public interface IEmailSender
{
	Task SendEmail(EmailAddress address, string subject, string message);
}
using Domain.Common.Results;
using Domain.Common.ValueObjects;
using FuncNet;

namespace Application.Email;

public interface IEmailSender
{
	Task<Result<Success, EmailSendingError>> SendEmail(
		EmailAddress address,
		string subject,
		string message,
		CancellationToken cancellationToken);
}
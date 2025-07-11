using Domain.Common.Results;
using Domain.Models.User;
using FuncNet;

namespace Application.Email;

public interface IUserEmailSender
{
	public Task<Result<Success, EmailSendingError>> SendEmailConfirmationLink(
		User user,
		EmailConfirmationCodeData data,
		CancellationToken cancellationToken);

	public Task<Result<Success, EmailSendingError>> SendPasswordResetLink(
		User user,
		PasswordResetCodeData data,
		CancellationToken cancellationToken);
}
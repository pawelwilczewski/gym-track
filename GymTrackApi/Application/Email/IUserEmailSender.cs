using Domain.Models.User;

namespace Application.Email;

public interface IUserEmailSender
{
	public Task SendEmailConfirmationLink(User user, EmailConfirmationCodeData data, CancellationToken cancellationToken);
	public Task SendPasswordResetLink(User user, PasswordResetCodeData data, CancellationToken cancellationToken);
}
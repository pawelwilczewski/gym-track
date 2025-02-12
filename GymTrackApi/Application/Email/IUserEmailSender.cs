using Domain.Models.User;

namespace Application.Email;

public interface IUserEmailSender
{
	public Task SendEmailConfirmationLink(User user, EmailConfirmationCodeData data, CancellationToken cancellationToken);
	public Task SendPasswordResetLink(User user, string resetLink, CancellationToken cancellationToken);
	public Task SendPasswordResetCode(User user, string resetCode, CancellationToken cancellationToken);
}
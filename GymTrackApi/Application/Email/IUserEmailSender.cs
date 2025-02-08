using Domain.Models.User;

namespace Application.Email;

public interface IUserEmailSender
{
	public Task SendConfirmationLink(User user, string confirmationLink);
	public Task SendPasswordResetLink(User user, string resetLink);
	public Task SendPasswordResetCode(User user, string resetCode);
}
using Application.Email;
using Domain.Models.User;

namespace Infrastructure.Email;

internal sealed class UserEmailSender : IUserEmailSender
{
	private readonly IEmailSender emailSender;

	public UserEmailSender(IEmailSender emailSender) => this.emailSender = emailSender;

	public Task SendConfirmationLink(User user, string confirmationLink) =>
		emailSender.SendEmail(
			user.Email,
			"Account Confirmation",
			$"Confirm your email by going to: {confirmationLink}");

	public Task SendPasswordResetLink(User user, string resetLink) =>
		emailSender.SendEmail(
			user.Email,
			"Password Reset Request",
			$"Reset your password by going to: {resetLink}");

	public Task SendPasswordResetCode(User user, string resetCode) =>
		emailSender.SendEmail(
			user.Email,
			"Password Reset Request",
			$"Use this code to reset your password: {resetCode}");
}
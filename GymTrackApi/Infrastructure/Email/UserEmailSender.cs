using Application.Email;
using Application.Settings;
using Domain.Common.Results;
using Domain.Models.User;
using FuncNet;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email;

internal sealed class UserEmailSender : IUserEmailSender
{
	private readonly IEmailSender emailSender;
	private readonly FrontendSettings frontendSettings;

	public UserEmailSender(IEmailSender emailSender, IOptions<FrontendSettings> frontendSettings)
	{
		this.emailSender = emailSender;
		this.frontendSettings = frontendSettings.Value;
	}

	public Task<Result<Success, EmailSendingError>> SendEmailConfirmationLink(
		User user,
		EmailConfirmationCodeData data,
		CancellationToken cancellationToken)
	{
		var confirmationLink = frontendSettings.BuildEmailConfirmationUrl(data.Code.Value);
		return emailSender.SendEmail(
			user.Email,
			"Account Confirmation",
			$"Confirm your email by going to: {confirmationLink}. This link will expire at: {data.ExpiresAt}.",
			cancellationToken);
	}

	public Task<Result<Success, EmailSendingError>> SendPasswordResetLink(
		User user,
		PasswordResetCodeData data,
		CancellationToken cancellationToken)
	{
		var resetLink = frontendSettings.BuildPasswordResetUrl(data.Code.Value);
		return emailSender.SendEmail(
			user.Email,
			"Password Reset Request",
			$"Reset your password by going to: {resetLink}",
			cancellationToken);
	}
}
// ReSharper disable UnusedAutoPropertyAccessor.Local

using System.Diagnostics.CodeAnalysis;
using Domain.Common.Results;
using Domain.Common.ValueObjects;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.User;

public class UserEmailConfirmationCode
{
	public UserId UserId { get; private set; }

	public User User { get; private set; }

	public EmailConfirmationCode EmailConfirmationCode { get; private set; }

	public EmailConfirmationCodeExpiryDateTime ExpiresAt { get; private set; }

	public EmailConfirmationCodeData Data => new(EmailConfirmationCode, ExpiresAt);

	private UserEmailConfirmationCode() { }

	private UserEmailConfirmationCode(User user, EmailConfirmationCodeData data)
	{
		User = user;
		UserId = user.Id;
		EmailConfirmationCode = data.Code;
		ExpiresAt = data.ExpiresAt;
	}

	public static bool TryCreate(
		User user,
		EmailConfirmationCodeData data,
		[NotNullWhen(true)] out UserEmailConfirmationCode? userEmailConfirmationCode,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (data.ExpiresAt.Value <= DateTime.UtcNow)
		{
			userEmailConfirmationCode = null;
			error = new ValidationError("Email confirmation code must expire in the future.");
			return false;
		}

		userEmailConfirmationCode = new UserEmailConfirmationCode(user, data);
		error = null;
		return true;
	}

	public bool IsCodeValid(EmailConfirmationCode code) =>
		DateTime.UtcNow < ExpiresAt.Value && code == EmailConfirmationCode;
}

public sealed record class EmailConfirmationCodeData(EmailConfirmationCode Code, EmailConfirmationCodeExpiryDateTime ExpiresAt);
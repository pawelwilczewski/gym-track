// ReSharper disable UnusedAutoPropertyAccessor.Local

using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.User;

public class UserPasswordResetCode
{
	public UserId UserId { get; private set; }

	public User User { get; private set; }

	public PasswordResetCode PasswordResetCode { get; private set; }

	public PasswordResetCodeExpiryDateTime ExpiresAt { get; private set; }

	public PasswordResetCodeData Data => new(PasswordResetCode, ExpiresAt);

	private UserPasswordResetCode() { }

	private UserPasswordResetCode(User user, PasswordResetCodeData data)
	{
		User = user;
		UserId = user.Id;
		PasswordResetCode = data.Code;
		ExpiresAt = data.ExpiresAt;
	}

	public static UserPasswordResetCode Create(User user, PasswordResetCodeData data) => new(user, data);

	public bool IsCodeValid(PasswordResetCode code) =>
		DateTime.UtcNow < ExpiresAt.Value && code == PasswordResetCode;

	public void Update(PasswordResetCodeData data, UserId userId)
	{
		if (userId != UserId) throw new PermissionError();

		PasswordResetCode = data.Code;
		ExpiresAt = data.ExpiresAt;
	}
}

public sealed record class PasswordResetCodeData(PasswordResetCode Code, PasswordResetCodeExpiryDateTime ExpiresAt);
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<DateTime>]
public readonly partial struct RefreshTokenExpiryDateTime
{
	private static Validation Validate(DateTime input) => input < DateTime.UtcNow
		? Validation.Invalid("Password reset code must expire in the future.")
		: Validation.Ok;
}
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<DateTime>(deserializationStrictness: DeserializationStrictness.DisallowNulls)]
public readonly partial struct PasswordResetCodeExpiryDateTime
{
	private static Validation Validate(DateTime input) => input < DateTime.UtcNow
		? Validation.Invalid("Password reset code must expire in the future.")
		: Validation.Ok;

	public static PasswordResetCodeExpiryDateTime CreateExpired() => __Deserialize(DateTime.MinValue);
}
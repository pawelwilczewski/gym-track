using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<DateTime>(deserializationStrictness: DeserializationStrictness.DisallowNulls)]
public readonly partial struct EmailConfirmationCodeExpiryDateTime
{
	private static Validation Validate(DateTime input) => input < DateTime.UtcNow
		? Validation.Invalid("Email confirmation code must expire in the future.")
		: Validation.Ok;

	public static EmailConfirmationCodeExpiryDateTime CreateExpired() => __Deserialize(DateTime.MinValue);
}
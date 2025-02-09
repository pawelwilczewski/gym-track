using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct PasswordHashOnly
{
	private static Validation Validate(string input) =>
		input.Length >= PasswordHash.HASH_BYTES_LENGTH
			? Validation.Ok
			: Validation.Invalid("Invalid password hash-only length.");
}
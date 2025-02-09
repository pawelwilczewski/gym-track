using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct PasswordSalt
{
	private static Validation Validate(string input) =>
		input.Length >= PasswordHash.SALT_BYTES_LENGTH
			? Validation.Ok
			: Validation.Invalid("Invalid password salt length.");
}
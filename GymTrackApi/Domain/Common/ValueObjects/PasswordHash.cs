using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct PasswordHash
{
	public const int SALT_BYTES_LENGTH = 16;
	public const int HASH_BYTES_LENGTH = 32;
	public const int BYTES_LENGTH = SALT_BYTES_LENGTH + HASH_BYTES_LENGTH;

	public const char DELIMITER = '_';

	public PasswordSalt Salt => PasswordSalt.From(Value.Split(DELIMITER, 2)[0]);

	public PasswordHashOnly HashOnly => PasswordHashOnly.From(Value.Split(DELIMITER, 2)[1]);

	private static Validation Validate(string input) => input.Length >= BYTES_LENGTH
		? Validation.Ok
		: Validation.Invalid("Invalid password hash length.");
}
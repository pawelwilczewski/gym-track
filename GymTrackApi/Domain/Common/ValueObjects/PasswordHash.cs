using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct PasswordHash
{
	public const int SALT_LENGTH = 16;
	public const int HASH_LENGTH = 32;
	public const int LENGTH = SALT_LENGTH + HASH_LENGTH;

	public PasswordSalt Salt => PasswordSalt.From(Value[..SALT_LENGTH]);
	public PasswordHashOnly HashOnly => PasswordHashOnly.From(Value[SALT_LENGTH..]);

	private static Validation Validate(string input) => input.Length == LENGTH
		? Validation.Ok
		: Validation.Invalid("Invalid password hash length.");
}
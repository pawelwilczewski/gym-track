using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct RefreshToken
{
	public const int BYTES_LENGTH = 16;

	private static Validation Validate(string input) =>
		input.Length >= BYTES_LENGTH
			? Validation.Ok
			: Validation.Invalid("Invalid refresh token length.");
}
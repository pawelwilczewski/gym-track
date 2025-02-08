using Domain.Common.ValidationExtensions;
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct Password
{
	public const int MIN_LENGTH = 6;
	public const int MAX_LENGTH = 128;
	public const int MIN_LOWER_CASE_COUNT = 1;
	public const int MIN_UPPER_CASE_COUNT = 0;
	public const int MIN_DIGIT_COUNT = 1;
	public const int MIN_SPECIAL_COUNT = 0;

	private static Validation Validate(string input)
	{
		if (!input.IsNotNullOrWhitespace(out var error)
			|| !input.HasMinLength(MIN_LENGTH, out error)
			|| !input.HasMaxLength(MAX_LENGTH, out error)
			|| !input.HasNoWhiteSpaceCharacters(out error)
			|| !input.HasMinLowerCaseCharacters(MIN_LOWER_CASE_COUNT, out error)
			|| !input.HasMinUpperCaseCharacters(MIN_UPPER_CASE_COUNT, out error)
			|| !input.HasMinDigitCharacters(MIN_DIGIT_COUNT, out error)
			|| !input.HasMinSpecialCharacters(MIN_SPECIAL_COUNT, out error))
		{
			return Validation.Invalid(error.Value.ErrorMessage);
		}

		return Validation.Ok;
	}
}
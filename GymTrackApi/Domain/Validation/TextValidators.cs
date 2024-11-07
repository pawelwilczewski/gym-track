namespace Domain.Validation;

public delegate bool TextValidator(string text, out TextValidationError error);

public static class TextValidators
{
	private static bool ValidateWithError(bool successCondition, string errorMessage, out TextValidationError error)
	{
		error = successCondition ? default : new TextValidationError(errorMessage);
		return successCondition;
	}

	private static bool IsNotNull(string? text, out TextValidationError error) =>
		ValidateWithError(text is not null, "Value required.", out error);

	private static bool IsNotNullOrWhitespace(string? text, out TextValidationError error) =>
		ValidateWithError(!string.IsNullOrWhiteSpace(text), "Cannot be empty.", out error);

	private static bool HasMinLength(string text, int minLength, out TextValidationError error)
	{
		if (minLength < 0)
		{
			error = new TextValidationError("Min length must be greater than or equal to 0 (internal error).");
			return false;
		}

		return ValidateWithError(
			text.Length >= minLength,
			$"Too short (min {minLength} characters).",
			out error);
	}

	private static bool HasMaxLength(string text, int maxLength, out TextValidationError error)
	{
		if (maxLength < 1)
		{
			error = new TextValidationError("Max length must be greater than 0 (internal error).");
			return false;
		}

		return ValidateWithError(
			text.Length <= maxLength,
			$"Too long (max {maxLength} characters).",
			out error);
	}

	private static bool IsNotJustPunctuation(string text, out TextValidationError error) => ValidateWithError(
		!text.All(c => char.IsPunctuation(c) || char.IsWhiteSpace(c)),
		"Cannot be just punctuation.",
		out error);

	public static bool Name(string text, out TextValidationError error) =>
		IsNotNullOrWhitespace(text, out error)
		&& HasMaxLength(text, Models.Name.MAX_LENGTH, out error)
		&& IsNotJustPunctuation(text, out error);

	public static bool Description(string text, out TextValidationError error) =>
		IsNotNull(text, out error)
		&& HasMaxLength(text, Models.Description.MAX_LENGTH, out error);

	public static bool FilePath(string text, out TextValidationError error) =>
		IsNotNullOrWhitespace(text, out error)
		&& HasMinLength(text, 1, out error)
		&& HasMaxLength(text, Models.FilePath.MAX_LENGTH, out error)
		&& IsNotJustPunctuation(text, out error);
}
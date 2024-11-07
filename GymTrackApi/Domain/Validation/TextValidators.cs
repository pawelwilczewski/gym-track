namespace Domain.Validation;

public delegate bool TextValidator(string text, out TextValidationError error);

public static class TextValidators
{
	private static bool IsNotNull(string? text, out TextValidationError error)
	{
		var success = text != null;
		error = success ? default : new TextValidationError("Value required.");
		return success;
	}

	private static bool IsNotNullOrWhitespace(string? text, out TextValidationError error)
	{
		var success = !string.IsNullOrWhiteSpace(text);
		error = success ? default : new TextValidationError("Value cannot be empty.");
		return success;
	}

	private static bool HasMinLength(string text, int minLength, out TextValidationError error)
	{
		if (minLength < 0)
		{
			error = new TextValidationError("Min length must be greater than or equal to 0 (internal error).");
			return false;
		}

		var success = text.Length >= minLength;
		error = success ? default : new TextValidationError($"Too short (min {minLength} characters).");
		return success;
	}

	private static bool HasMaxLength(string text, int maxLength, out TextValidationError error)
	{
		if (maxLength < 1)
		{
			error = new TextValidationError("Max length must be greater than 0 (internal error).");
			return false;
		}

		var success = text.Length <= maxLength;
		error = success ? default : new TextValidationError($"Too long (max {maxLength} characters).");
		return success;
	}

	private static bool IsNotJustPunctuation(string text, out TextValidationError error)
	{
		var success = !text.All(c => char.IsPunctuation(c) || char.IsWhiteSpace(c));
		error = success ? default : new TextValidationError("Cannot be just punctuation.");
		return success;
	}

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
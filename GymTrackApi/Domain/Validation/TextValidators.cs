namespace Domain.Validation;

public delegate TextValidationResult TextValidator(string text);

public static class TextValidators
{
	private static TextValidationResult NotNull(string? text) => text == null
		? new TextValidationResult.Invalid("Text is required.")
		: new TextValidationResult.Success();

	private static TextValidationResult NotNullOrWhitespace(string text) => string.IsNullOrWhiteSpace(text)
		? new TextValidationResult.Invalid("Text cannot be empty.")
		: new TextValidationResult.Success();

	private static TextValidationResult MinLength(string text, int minLength)
	{
		if (minLength < 0) return new TextValidationResult.Invalid("Min length must be greater than or equal to 0 (internal error).");

		return text.Length < minLength
			? new TextValidationResult.Invalid($"Text too short (min {minLength} characters).")
			: new TextValidationResult.Success();
	}

	private static TextValidationResult MaxLength(string text, int maxLength)
	{
		if (maxLength < 1) return new TextValidationResult.Invalid("Max length must be greater than 0 (internal error).");

		return text.Length > maxLength
			? new TextValidationResult.Invalid($"Text too long (max {maxLength} characters).")
			: new TextValidationResult.Success();
	}

	private static TextValidationResult NotJustPunctuation(string text) =>
		text.All(c => char.IsPunctuation(c) || char.IsWhiteSpace(c))
			? new TextValidationResult.Invalid("Text can't be just punctuation.")
			: new TextValidationResult.Success();

	public static TextValidationResult Name(string text)
	{
		if (NotNullOrWhitespace(text) is TextValidationResult.Invalid invalid1) return invalid1;
		if (MaxLength(text, Models.Name.MAX_LENGTH) is TextValidationResult.Invalid invalid2) return invalid2;
		if (NotJustPunctuation(text) is TextValidationResult.Invalid invalid3) return invalid3;

		return new TextValidationResult.Success();
	}

	public static TextValidationResult Description(string text)
	{
		if (NotNull(text) is TextValidationResult.Invalid invalid1) return invalid1;
		if (MaxLength(text, Models.Description.MAX_LENGTH) is TextValidationResult.Invalid invalid2) return invalid2;

		return new TextValidationResult.Success();
	}

	public static TextValidationResult FilePath(string text)
	{
		if (NotNullOrWhitespace(text) is TextValidationResult.Invalid invalid1) return invalid1;
		if (MinLength(text, 1) is TextValidationResult.Invalid invalid2) return invalid2;
		if (MaxLength(text, Models.FilePath.MAX_LENGTH) is TextValidationResult.Invalid invalid3) return invalid3;
		if (NotJustPunctuation(text) is TextValidationResult.Invalid invalid4) return invalid4;

		return new TextValidationResult.Success();
	}
}
namespace Domain.Validation;

public delegate TextValidationResult TextValidator(string text);

public static class TextValidators
{
	private static TextValidationResult NotEmptyOrWhitespace(string text) => string.IsNullOrWhiteSpace(text)
		? new TextValidationResult.Invalid("Text cannot be empty or whitespace.")
		: new TextValidationResult.Success();

	private static TextValidationResult MaxLength(string text, int maxLength)
	{
		if (maxLength < 1) return new TextValidationResult.Invalid("Max length must be greater than 0.");

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
		if (NotEmptyOrWhitespace(text) is TextValidationResult.Invalid invalid1) return invalid1;
		if (MaxLength(text, 50) is TextValidationResult.Invalid invalid2) return invalid2;
		if (NotJustPunctuation(text) is TextValidationResult.Invalid invalid3) return invalid3;

		return new TextValidationResult.Success();
	}

	public static TextValidationResult Description(string text) => MaxLength(text, 2000);
}
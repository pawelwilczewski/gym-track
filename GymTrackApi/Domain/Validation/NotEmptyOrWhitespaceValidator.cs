namespace Domain.Validation;

internal readonly record struct NotEmptyOrWhitespaceValidator : ITextValidator
{
	public TextValidationResult Validate(string text) => string.IsNullOrWhiteSpace(text)
		? new TextValidationResult.Invalid("Text cannot be empty or whitespace.")
		: new TextValidationResult.Success();
}
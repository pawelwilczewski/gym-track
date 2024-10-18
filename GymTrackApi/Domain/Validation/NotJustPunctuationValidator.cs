namespace Domain.Validation;

internal readonly record struct NotJustPunctuationValidator : ITextValidator
{
	public TextValidationResult Validate(string text) => text.All(c => char.IsPunctuation(c) || char.IsWhiteSpace(c))
		? new TextValidationResult.Invalid("Text can't be just punctuation.")
		: new TextValidationResult.Success();
}
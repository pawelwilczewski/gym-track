namespace Domain.Validation;

public readonly record struct NameValidator : ITextValidator
{
	private const int MAX_LENGTH = 50;

	private readonly NotEmptyOrWhitespaceValidator notEmptyOrWhitespaceValidator;
	private readonly MaxLengthValidator maxLengthValidator;
	private readonly NotJustPunctuationValidator notJustPunctuationValidator;

	public NameValidator()
	{
		notEmptyOrWhitespaceValidator = new NotEmptyOrWhitespaceValidator();
		maxLengthValidator = new MaxLengthValidator(MAX_LENGTH);
		notJustPunctuationValidator = new NotJustPunctuationValidator();
	}

	public TextValidationResult Validate(string text)
	{
		if (notEmptyOrWhitespaceValidator.Validate(text) is TextValidationResult.Invalid invalid1) return invalid1;
		if (maxLengthValidator.Validate(text) is TextValidationResult.Invalid invalid2) return invalid2;
		if (notJustPunctuationValidator.Validate(text) is TextValidationResult.Invalid invalid3) return invalid3;

		return new TextValidationResult.Success();
	}
}
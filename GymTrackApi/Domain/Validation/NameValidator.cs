namespace Domain.Validation;

public readonly record struct NameValidator : ITextValidator
{
	public int MaxLength => maxLengthValidator.Length;

	private readonly NotEmptyOrWhitespaceValidator notEmptyOrWhitespaceValidator;
	private readonly MaxLengthValidator maxLengthValidator;
	private readonly NotJustPunctuationValidator notJustPunctuationValidator;

	public NameValidator(int maxLength)
	{
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxLength, 0, nameof(maxLength));

		notEmptyOrWhitespaceValidator = new NotEmptyOrWhitespaceValidator();
		maxLengthValidator = new MaxLengthValidator(maxLength);
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
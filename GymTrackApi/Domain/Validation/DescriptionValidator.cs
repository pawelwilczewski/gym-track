namespace Domain.Validation;

public readonly record struct DescriptionValidator : ITextValidator
{
	public int MaxLength => maxLengthValidator.Length;

	private readonly MaxLengthValidator maxLengthValidator;

	public DescriptionValidator(int maxLength)
	{
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxLength, 0, nameof(maxLength));

		maxLengthValidator = new MaxLengthValidator(maxLength);
	}

	public TextValidationResult Validate(string text) => maxLengthValidator.Validate(text);
}
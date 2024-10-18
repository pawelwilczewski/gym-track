namespace Domain.Validation;

public readonly record struct DescriptionValidator : ITextValidator
{
	private const int MAX_LENGTH = 2000;

	private readonly MaxLengthValidator maxLengthValidator;

	public DescriptionValidator() => maxLengthValidator = new MaxLengthValidator(MAX_LENGTH);

	public TextValidationResult Validate(string text) => maxLengthValidator.Validate(text);
}
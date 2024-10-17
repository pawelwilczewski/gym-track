namespace Domain.Validation;

internal readonly record struct MaxLengthValidator : IValidator
{
	private int Length { get; }

	public MaxLengthValidator(int length) => Length = length > 0
		? length
		: throw new ArgumentOutOfRangeException(nameof(length));

	public TextValidationResult Validate(string text) => text.Length > Length
		? new TextValidationResult.Invalid($"Text too long (max {Length} characters).")
		: new TextValidationResult.Success();
}
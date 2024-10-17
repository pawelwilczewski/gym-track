namespace Domain.Validation;

public abstract record class TextValidationResult
{
	public sealed record class Success : TextValidationResult;

	public sealed record class Invalid(string Error) : TextValidationResult;
}
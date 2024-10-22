namespace Domain.Validation;

public abstract record class TextValidationResult
{
	public sealed record class Success : TextValidationResult; // TODO Pawel: remove success, similar to CantModifyReason

	public sealed record class Invalid(string Error) : TextValidationResult;
}
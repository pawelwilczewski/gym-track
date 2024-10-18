namespace Domain.Validation;

public interface ITextValidator
{
	TextValidationResult Validate(string text);
}
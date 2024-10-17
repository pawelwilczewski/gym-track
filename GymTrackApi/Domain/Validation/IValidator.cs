namespace Domain.Validation;

public interface IValidator
{
	TextValidationResult Validate(string text);
}
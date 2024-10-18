using Domain.Validation;

namespace Domain.Models;

public sealed record class Description() : ValidatedText<Description>(string.Empty)
{
	protected override TextValidator Validator => TextValidators.Description;

	public static TextValidationResult TryCreate(string value, out Description description)
	{
		description = new Description();
		return description.Set(value);
	}
}
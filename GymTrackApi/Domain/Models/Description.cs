using Domain.Validation;

namespace Domain.Models;

public sealed record class Description : ValidatedText<Description>
{
	public const int MAX_LENGTH = 2000;

	protected override TextValidator Validator => TextValidators.Description;

	private Description(string value = "") : base(value) { }

	public static TextValidationResult TryCreate(string value, out Description description)
	{
		description = new Description();
		return description.Set(value);
	}

	public override string ToString() => base.ToString();
}
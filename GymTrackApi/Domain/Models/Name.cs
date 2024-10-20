using Domain.Validation;

namespace Domain.Models;

public sealed record class Name : ValidatedText<Name>
{
	public const int MAX_LENGTH = 50;

	protected override TextValidator Validator => TextValidators.Name;

	private Name(string value = "") : base(value) { }

	public static TextValidationResult TryCreate(string value, out Name name)
	{
		name = new Name();
		return name.Set(value);
	}

	public override string ToString() => base.ToString();
}
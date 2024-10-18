using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Validation;

public abstract record class ValidatedText<T> where T : ValidatedText<T>, new()
{
	public static ValueConverter<T, string> Converter { get; } = new(
		description => description.Value,
		value => new T
		{
			Value = value
		});

	private string Value { get; set; }
	protected abstract TextValidator Validator { get; }

	protected ValidatedText(string value) => Value = value;

	public TextValidationResult Set(string value)
	{
		value = value.Trim();

		switch (Validator(value))
		{
			case TextValidationResult.Invalid invalid: return invalid;
			case TextValidationResult.Success:
			{
				Value = value;
				return new TextValidationResult.Success();
			}
			default: throw new UnreachableException();
		}
	}
}
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Validation;

public abstract record class ValidatedText<T> where T : ValidatedText<T>, new()
{
	public static ValueConverter<T, string> Converter { get; } = new(
		text => text.Value,
		value => new T
		{
			Value = value
		});

	public static ValueComparer<T> Comparer { get; } = new(
		(a, b) => (a == null && b == null) || (a != null && b != null && a.Value == b.Value),
		value => value.GetHashCode(),
		text => new T
		{
			Value = text.Value
		});

	private string Value { get; set; }
	protected abstract TextValidator Validator { get; }

	protected ValidatedText(string value) => Value = value;

	public TextValidationResult Set(string value)
	{
		switch (Validator(value))
		{
			case TextValidationResult.Invalid invalid: return invalid;
			case TextValidationResult.Success:
			{
				Value = value.Trim();
				return new TextValidationResult.Success();
			}
			default: throw new UnreachableException();
		}
	}
}
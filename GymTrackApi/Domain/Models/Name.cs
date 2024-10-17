using Domain.Validation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public readonly record struct Name
{
	public static ValueConverter<Name, string> Converter { get; } = new(name => name.Value, value => new Name(value));

	private string Value { get; }

	private Name(string value) => Value = value;

	public static TextValidationResult TryCreate(string value, out Name name)
	{
		name = default;
		if (string.IsNullOrWhiteSpace(value))
		{
			return new TextValidationResult.Invalid("Name cannot be empty.");
		}

		value = value.Trim();
		if (value.All(c => char.IsPunctuation(c) || char.IsWhiteSpace(c)))
		{
			return new TextValidationResult.Invalid("Name can't be just punctuation.");
		}

		name = new Name(value);
		return new TextValidationResult.Success();
	}
}
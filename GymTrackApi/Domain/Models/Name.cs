using System.Diagnostics;
using Domain.Validation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public record struct Name
{
	public static ValueConverter<Name, string> Converter { get; } = new(
		name => name.Value,
		value => new Name(value));

	private string Value { get; set; }

	private NameValidator Validator { get; }

	private Name(string value)
	{
		Value = value;
		Validator = new NameValidator();
	}

	public TextValidationResult Set(string value)
	{
		value = value.Trim();

		switch (Validator.Validate(value))
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

	public static TextValidationResult TryCreate(string value, out Name name)
	{
		name = new Name();
		return name.Set(value);
	}
}
using System.Diagnostics;
using Domain.Validation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public record struct Description
{
	public static ValueConverter<Description, string> Converter { get; } = new(
		description => description.Value,
		value => new Description(value));

	private string Value { get; set; }

	private DescriptionValidator Validator { get; }

	private Description(string value)
	{
		Value = value;
		Validator = new DescriptionValidator();
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

	public static TextValidationResult TryCreate(string value, out Description description)
	{
		description = new Description();
		return description.Set(value);
	}
}
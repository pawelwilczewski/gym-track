using System.Diagnostics;
using Domain.Validation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public record struct Description
{
	public static ValueConverter<Description, string> Converter { get; } = new(
		description => Serialize(description),
		value => Deserialize(value));

	private string Value { get; set; }

	private DescriptionValidator Validator { get; set; }

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

	public static TextValidationResult TryCreate(string value, int maxLength, out Description description)
	{
		description = default;
		if (maxLength < 1) return new TextValidationResult.Invalid("Max length must be greater than 0.");

		description.Validator = new DescriptionValidator(maxLength);

		return description.Set(value);
	}

	private static string Serialize(Description description) =>
		$"{description.Validator.MaxLength}|{description.Value}";

	private static Description Deserialize(string value)
	{
		var split = value.Split('|', 2);
		return new Description
		{
			Validator = new DescriptionValidator(int.Parse(split[0])),
			Value = split[1]
		};
	}
}
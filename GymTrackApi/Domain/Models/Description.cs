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
	private MaxLengthValidator MaxLengthValidator { get; set; }

	public TextValidationResult Set(string value)
	{
		value = value.Trim();

		switch (MaxLengthValidator.Validate(value))
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

		description.MaxLengthValidator = new MaxLengthValidator(maxLength);

		return description.Set(value);
	}

	private static string Serialize(Description description) =>
		$"{description.MaxLengthValidator.Length}|{description.Value}";

	private static Description Deserialize(string value)
	{
		var split = value.Split('|', 2);
		return new Description
		{
			MaxLengthValidator = new MaxLengthValidator(int.Parse(split[0])),
			Value = split[1]
		};
	}
}
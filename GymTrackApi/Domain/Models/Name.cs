using System.Diagnostics;
using Domain.Validation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public record struct Name
{
	public static ValueConverter<Name, string> Converter { get; } = new(
		description => Serialize(description),
		value => Deserialize(value));

	private string Value { get; set; }

	private NameValidator Validator { get; set; }

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

	public static TextValidationResult TryCreate(string value, int maxLength, out Name name)
	{
		name = default;
		if (maxLength < 1) return new TextValidationResult.Invalid("Max length must be greater than 0.");

		name.Validator = new NameValidator(maxLength);

		return name.Set(value);
	}

	private static string Serialize(Name name) =>
		$"{name.Validator.MaxLength}|{name.Value}";

	private static Name Deserialize(string value)
	{
		var split = value.Split('|', 2);
		return new Name
		{
			Validator = new NameValidator(int.Parse(split[0])),
			Value = split[1]
		};
	}
}
using Domain.Validation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public record struct Description
{
	public static ValueConverter<Description, string> Converter { get; } = new(
		description => Serialize(description),
		value => Deserialize(value));

	private string Value { get; set; }
	private int MaxLength { get; set; }

	public TextValidationResult Set(string value)
	{
		value = value.Trim();

		if (value.Length > MaxLength) return new TextValidationResult.Invalid($"Description too long (max {MaxLength} characters)");

		Value = value;
		return new TextValidationResult.Success();
	}

	public static TextValidationResult TryCreate(string value, int maxLength, out Description description)
	{
		description = default;
		if (maxLength < 1) return new TextValidationResult.Invalid("Max length must be greater than 0.");

		description.MaxLength = maxLength;

		return description.Set(value);
	}

	private static string Serialize(Description description) =>
		$"{description.MaxLength}|{description.Value}";

	private static Description Deserialize(string value)
	{
		var split = value.Split('|', 2);
		return new Description
		{
			MaxLength = int.Parse(split[0]),
			Value = split[1]
		};
	}
}
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public record struct Description
{
	public static ValueConverter<Description, string> Converter { get; } = new(description => description.Value, value => new Description(value));

	private string Value { get; }

	private Description(string value) => Value = value;

	public static bool TryCreate(string value, out Description description)
	{
		description = new Description(value.Trim());
		return true;
	}
}
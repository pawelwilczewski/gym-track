using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public readonly record struct Name
{
	public static ValueConverter<Name, string> Converter { get; } = new(name => name.Value, value => new Name(value));

	private string Value { get; }

	private Name(string value) => Value = value;

	public static bool TryCreate(string value, out Name name)
	{
		name = default;
		if (string.IsNullOrWhiteSpace(value)) return false;

		value = value.Trim();
		if (value.All(c => char.IsPunctuation(c) || char.IsWhiteSpace(c))) return false;

		name = new Name(value);
		return true;
	}
}
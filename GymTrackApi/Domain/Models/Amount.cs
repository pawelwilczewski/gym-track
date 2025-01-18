using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

internal sealed class AmountJsonConverter : JsonConverter<Amount>
{
	public override Amount Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.Number)
		{
			throw new JsonException($"Expected number when converting to Amount. Got {reader.TokenType}.");
		}

		var value = reader.GetDouble();
		Amount.TryCreate(value, out var amount);
		return amount;
	}

	public override void Write(Utf8JsonWriter writer, Amount value, JsonSerializerOptions options) =>
		writer.WriteNumberValue(value.Value);
}

[JsonConverter(typeof(AmountJsonConverter))]
public readonly record struct Amount : IParsable<Amount>, IEquatable<double>
{
	public static ValueConverter<Amount, double> Converter { get; } = new(id => id.Value, value => new Amount(value));

	public double Value { get; }

	private Amount(double value) => Value = value;

	public static Amount Parse(string s, IFormatProvider? provider) => new(double.Parse(s, provider));

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Amount result)
	{
		var success = double.TryParse(s, provider, out var value) && value >= 0;
		result = success ? new Amount(value) : new Amount(0);
		return success;
	}

	public static bool TryCreate(double value, out Amount result)
	{
		if (value < 0)
		{
			result = new Amount(0);
			return false;
		}

		result = new Amount(value);
		return true;
	}

	public static bool operator ==(Amount left, double right) => left.Equals(right);
	public static bool operator !=(Amount left, double right) => !left.Equals(right);

	public bool Equals(Amount other) => Math.Abs(Value - other.Value) <= double.Epsilon;
	public bool Equals(double other) => Math.Abs(Value - other) <= double.Epsilon;

	public override int GetHashCode()
	{
		var data = BitConverter.GetBytes(Value);
		return BitConverter.ToInt32(data, 0) ^ BitConverter.ToInt32(data, 4);
	}

	public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);
}
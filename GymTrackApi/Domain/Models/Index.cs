using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public readonly record struct Index : IParsable<Index>, IEquatable<uint>, IEquatable<int>
{
	public static ValueConverter<Index, int> Converter { get; } = new(id => (int)id.Value, value => new Index((uint)value));

	public static Index Parse(string s, IFormatProvider? provider) => new(uint.Parse(s, provider));

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Index result)
	{
		var success = uint.TryParse(s, provider, out var value);
		result = success ? new Index(value) : new Index(0);
		return success;
	}

	public static bool TryCreate(int value, out Index index)
	{
		if (value < 0)
		{
			index = default;
			return false;
		}

		index = new Index((uint)value);
		return true;
	}

	public static bool operator ==(Index left, uint right) => left.Equals(right);
	public static bool operator !=(Index left, uint right) => !left.Equals(right);

	public static bool operator ==(Index left, int right) => left.Equals(right);
	public static bool operator !=(Index left, int right) => !left.Equals(right);

	public uint Value { get; }
	public int IntValue => (int)Value;

	private Index(uint value) => Value = value;

	public override string ToString() => Value.ToString();

	public bool Equals(Index other) => Value == other.Value;
	public bool Equals(uint other) => Value == other;
	public bool Equals(int other) => Value == other;

	public override int GetHashCode() => (int)Value;
}
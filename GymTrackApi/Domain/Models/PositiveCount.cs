using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public readonly record struct PositiveCount : IParsable<PositiveCount>, IEquatable<uint>, IEquatable<int>
{
	public static ValueConverter<PositiveCount, int> Converter { get; } = new(id => (int)id.Value, value => new PositiveCount((uint)value));

	public uint Value { get; }
	public int IntValue => (int)Value;

	private PositiveCount(uint value) => Value = value;

	public static PositiveCount Parse(string s, IFormatProvider? provider) => new(uint.Parse(s, provider));

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out PositiveCount result)
	{
		var success = uint.TryParse(s, provider, out var value) && value >= 1;
		result = success ? new PositiveCount(value) : new PositiveCount(1);
		return success;
	}

	public static bool TryCreate(int value, out PositiveCount result)
	{
		if (value < 1)
		{
			result = new PositiveCount(1);
			return false;
		}

		result = new PositiveCount((uint)value);
		return true;
	}

	public static bool operator ==(PositiveCount left, uint right) => left.Equals(right);
	public static bool operator !=(PositiveCount left, uint right) => !left.Equals(right);

	public static bool operator ==(PositiveCount left, int right) => left.Equals(right);
	public static bool operator !=(PositiveCount left, int right) => !left.Equals(right);

	public bool Equals(PositiveCount other) => Value == other.Value;
	public bool Equals(uint other) => Value == other;
	public bool Equals(int other) => Value == other;

	public override int GetHashCode() => (int)Value;

	public override string ToString() => Value.ToString();
}
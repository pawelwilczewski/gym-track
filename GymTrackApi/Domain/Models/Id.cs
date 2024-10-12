using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Domain.Models;

public readonly record struct Id<T>(Guid Value) : IParsable<Id<T>>
{
	public static ValueConverter<Id<T>, Guid> Converter { get; } = new(id => id.Value, value => new Id<T>(value));

	public static Id<T> Empty { get; } = default;
	public static Id<T> New() => new(Ulid.NewUlid().ToGuid());

	public static Id<T> Parse(string s, IFormatProvider? provider) => new(Guid.Parse(s, provider));

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Id<T> result)
	{
		var success = Guid.TryParse(s, provider, out var guid);
		result = success ? new Id<T>(guid) : Empty;
		return success;
	}
}
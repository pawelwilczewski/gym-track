using System.Diagnostics.CodeAnalysis;

namespace Domain.Common;

// TODO Pawel: reconsider using this where I'm using it because it makes the code ridiculous - null would read better
public sealed class Option<T> : IEquatable<Option<T>> where T : class
{
	private readonly T? value;

	private Option(T? value) => this.value = value;
	public static Option<T> Some(T value) => new(value);
	public static Option<T> None() => new(null);

	public static bool operator ==(Option<T>? left, Option<T>? right) => Equals(left, right);
	public static bool operator !=(Option<T>? left, Option<T>? right) => !(left == right);

	public Option<TResult> Map<TResult>(Func<T, TResult> map) where TResult : class =>
		new(value is not null ? map(value) : null);

	[return: NotNullIfNotNull(nameof(@default))]
	public T? Reduce(T? @default) => value ?? @default;

	public bool Equals(Option<T>? other) =>
		other is not null
		&& (ReferenceEquals(this, other)
			|| (value is null && other.value is null)
			|| (value is not null && value.Equals(other.value)));

	public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is Option<T> other && Equals(other));
	public override int GetHashCode() => value?.GetHashCode() ?? 0;
}
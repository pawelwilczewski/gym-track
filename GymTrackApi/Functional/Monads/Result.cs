using System.Runtime.CompilerServices;
using OneOf;

namespace Functional.Monads;

[GenerateOneOf]
public partial class Result<TSuccess, TError> : OneOfBase<Success<TSuccess>, Error<TError>>
{
	public bool IsSuccess => Match(success => true, error => false);

	public static implicit operator Result<TSuccess, TError>(TSuccess value) => new(new Success<TSuccess>(value));
	public static implicit operator Result<TSuccess, TError>(TError errorValue) => new(new Error<TError>(errorValue));
}

public readonly record struct Success<TValue>(TValue Value)
{
	public static async Task<Result<TValue, TError>> FromAsync<TError>(Task<TValue> task) => new Success<TValue>(await task);
	public static async Task<Result<TValue, TError>> FromAsync<TError>(ConfiguredTaskAwaitable<TValue> task) => new Success<TValue>(await task);

	public static implicit operator Success<TValue>(TValue value) => new(value);
	public static implicit operator TValue(Success<TValue> success) => success.Value;
}

public readonly record struct Error<TError>(TError ErrorValue)
{
	public static implicit operator Error<TError>(TError errorValue) => new(errorValue);
	public static implicit operator TError(Error<TError> error) => error.ErrorValue;
}
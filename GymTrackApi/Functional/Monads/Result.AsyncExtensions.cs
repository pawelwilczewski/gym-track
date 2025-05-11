namespace Functional.Monads;

public static class ResultAsyncExtensions
{
	public static async Task<Result<TSuccessNew, TError>> MapAsync<TSuccessOld, TSuccessNew, TError>(
		this Task<Result<TSuccessOld, TError>> result,
		Func<TSuccessOld, TSuccessNew> mapping) => (await result).Map(mapping);

	public static async Task<Result<TSuccessNew, TError>> MapAsync<TSuccessOld, TSuccessNew, TError>(
		this Result<TSuccessOld, TError> result,
		Func<TSuccessOld, Task<TSuccessNew>> mapping) => result.IsSuccess
		? await mapping(result.AsT0.Value)
		: result.AsT1.ErrorValue;

	public static async Task<Result<TSuccessNew, TError>> MapAsync<TSuccessOld, TSuccessNew, TError>(
		this Task<Result<TSuccessOld, TError>> result,
		Func<TSuccessOld, Task<TSuccessNew>> mapping) => await (await result).MapAsync(mapping);

	public static async Task<Result<TSuccessNew, TError>> BindAsync<TSuccessOld, TSuccessNew, TError>(
		this Task<Result<TSuccessOld, TError>> result,
		Func<TSuccessOld, Result<TSuccessNew, TError>> binding) => (await result).Bind(binding);

	public static async Task<Result<TSuccessNew, TError>> BindAsync<TSuccessOld, TSuccessNew, TError>(
		this Result<TSuccessOld, TError> result,
		Func<TSuccessOld, Task<Result<TSuccessNew, TError>>> binding) => result.IsSuccess
		? await binding(result.AsT0.Value)
		: result.AsT1.ErrorValue;

	public static async Task<Result<TSuccessNew, TError>> BindAsync<TSuccessOld, TSuccessNew, TError>(
		this Task<Result<TSuccessOld, TError>> result,
		Func<TSuccessOld, Task<Result<TSuccessNew, TError>>> binding) => await (await result).BindAsync(binding);

	public static async Task<Result<TSuccess, TErrorNew>> MapErrorAsync<TSuccess, TErrorOld, TErrorNew>(
		this Task<Result<TSuccess, TErrorOld>> result,
		Func<TErrorOld, TErrorNew> errorMapping) => (await result).MapError(errorMapping);

	public static async Task<Result<TSuccess, TErrorNew>> MapErrorAsync<TSuccess, TErrorOld, TErrorNew>(
		this Result<TSuccess, TErrorOld> result,
		Func<TErrorOld, Task<TErrorNew>> errorMapping) => result.IsSuccess
		? result.AsT0.Value
		: await errorMapping(result.AsT1.ErrorValue);

	public static async Task<Result<TSuccess, TErrorNew>> MapErrorAsync<TSuccess, TErrorOld, TErrorNew>(
		this Task<Result<TSuccess, TErrorOld>> result,
		Func<TErrorOld, Task<TErrorNew>> errorMapping) => await (await result).MapErrorAsync(errorMapping);
}
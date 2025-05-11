namespace Functional.Monads;

public static class ResultExtensions
{
	public static Option<TSuccess> ToOption<TSuccess, TError>(this Result<TSuccess, TError> result) => result.Match<Option<TSuccess>>(
		success => success.Value,
		error => new None());

	public static Result<TSuccessNew, TError> Map<TSuccessOld, TSuccessNew, TError>(
		this Result<TSuccessOld, TError> result,
		Func<TSuccessOld, TSuccessNew> mapping) => result.Match<Result<TSuccessNew, TError>>(
		success => mapping(success.Value),
		error => error.ErrorValue);

	public static Result<TSuccessNew, TError> Bind<TSuccessOld, TSuccessNew, TError>(
		this Result<TSuccessOld, TError> result,
		Func<TSuccessOld, Result<TSuccessNew, TError>> binding) => result.Match<Result<TSuccessNew, TError>>(
		success => binding(success.Value),
		error => error.ErrorValue);

	public static Result<TSuccess, TErrorNew> MapError<TSuccess, TErrorOld, TErrorNew>(
		this Result<TSuccess, TErrorOld> result,
		Func<TErrorOld, TErrorNew> errorMapping) => result.Match<Result<TSuccess, TErrorNew>>(
		success => success.Value,
		error => errorMapping(error.ErrorValue));
}
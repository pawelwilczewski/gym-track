namespace Functional.Monads;

public static class ResultExtensions
{
	public static Option<TSuccess> ToOption<TSuccess, TError>(this Result<TSuccess, TError> result) => result.Match<Option<TSuccess>>(
		success => new Option<TSuccess>.Some(success.Value),
		error => new Option<TSuccess>.None());

	public static Result<TSuccessNew, TError> Map<TSuccessOld, TSuccessNew, TError>(
		this Result<TSuccessOld, TError> result,
		Func<TSuccessOld, TSuccessNew> mapping) => result.Match<Result<TSuccessNew, TError>>(
		success => new Result<TSuccessNew, TError>.Success(mapping(success.Value)),
		error => new Result<TSuccessNew, TError>.Error(error.ErrorValue));

	public static Result<TSuccessNew, TError> Bind<TSuccessOld, TSuccessNew, TError>(
		this Result<TSuccessOld, TError> result,
		Func<TSuccessOld, Result<TSuccessNew, TError>> binding) => result.Match<Result<TSuccessNew, TError>>(
		success => binding(success.Value),
		error => new Result<TSuccessNew, TError>.Error(error.ErrorValue));

	public static Result<TSuccess, TErrorNew> MapError<TSuccess, TErrorOld, TErrorNew>(
		this Result<TSuccess, TErrorOld> result,
		Func<TErrorOld, TErrorNew> errorMapping) => result.Match<Result<TSuccess, TErrorNew>>(
		success => new Result<TSuccess, TErrorNew>.Success(success.Value),
		error => new Result<TSuccess, TErrorNew>.Error(errorMapping(error.ErrorValue)));
}
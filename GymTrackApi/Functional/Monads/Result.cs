using System.Runtime.CompilerServices;
using Dunet;

namespace Functional.Monads;

[Union]
public abstract partial record class Result<TSuccess, TError>
{
	public abstract bool IsSuccess { get; }

	public sealed partial record class Success(TSuccess Value)
	{
		public override bool IsSuccess => true;

		public static async Task<Result<TSuccess, TError>> FromAsync(Task<TSuccess> task) => new Success(await task);
		public static async Task<Result<TSuccess, TError>> FromAsync(ConfiguredTaskAwaitable<TSuccess> task) => new Success(await task);
	}

	public sealed partial record class Error(TError ErrorValue)
	{
		public override bool IsSuccess => false;
	}
}
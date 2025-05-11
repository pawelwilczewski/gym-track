namespace Functional.Monads;

public static class OptionExtensions
{
	public static Option<TValue> Map<TValue>(this Option<TValue> option, Func<TValue, TValue> mapping) => option.Match<Option<TValue>>(
		some => mapping(some.Value),
		none => none);

	public static Option<TValue> Bind<TValue>(this Option<TValue> option, Func<TValue, Option<TValue>> binding) => option.Match<Option<TValue>>(
		some => binding(some.Value),
		none => none);
}
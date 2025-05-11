using OneOf;

namespace Functional.Monads;

// [Union]
// public partial record class Option<TValue>
// {
// 	public partial record class Some(TValue Value);
//
// 	public partial record class None;
// }

[GenerateOneOf]
public sealed partial class Option<TValue> : OneOfBase<Some<TValue>, None>
{
	public static implicit operator Option<TValue>(TValue value) => new Some<TValue>(value);
}

public readonly record struct Some<TValue>(TValue Value)
{
	public static implicit operator Some<TValue>(TValue value) => new(value);
	public static implicit operator TValue(Some<TValue> value) => value.Value;
}

public readonly record struct None;
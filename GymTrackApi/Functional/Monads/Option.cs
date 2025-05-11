using Dunet;

namespace Functional.Monads;

[Union]
public partial record class Option<TValue>
{
	public partial record class Some(TValue Value);

	public partial record class None;
}
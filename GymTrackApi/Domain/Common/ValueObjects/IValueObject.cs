namespace Domain.Common.ValueObjects;

public interface IValueObject<TPrimitive, out TSelf>
{
	TPrimitive Value { get; }

	static abstract TSelf From(TPrimitive value);
}
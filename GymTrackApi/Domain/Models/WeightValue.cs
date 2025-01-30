using Vogen;

namespace Domain.Models;

[ValueObject<double>(Conversions.SystemTextJson, parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces)]
public readonly partial struct WeightValue
{
	private static Validation Validate(double input) =>
		input < 0
			? Validation.Invalid("Can't create a negative weight value.")
			: Validation.Ok;
}
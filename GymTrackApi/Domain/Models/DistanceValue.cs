using Vogen;

namespace Domain.Models;

[ValueObject<double>(Conversions.SystemTextJson, parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces)]
public readonly partial struct DistanceValue
{
	private static Validation Validate(double input) =>
		input < 0
			? Validation.Invalid("Can't create a negative distance value.")
			: Validation.Ok;
}
using Vogen;

namespace Domain.Models;

[ValueObject<double>(Conversions.SystemTextJson, parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces)]
public readonly partial struct Amount
{
	private static double NormalizeInput(double input) => input;

	private static Validation Validate(double input) =>
		input < 0
			? Validation.Invalid("Can't create a negative amount.")
			: Validation.Ok;
}
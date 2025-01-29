using Vogen;

namespace Domain.Models;

[ValueObject<int>(
	parsableForPrimitives: ParsableForPrimitives.HoistMethodsAndInterfaces,
	primitiveEqualityGeneration: PrimitiveEqualityGeneration.GenerateOperatorsAndMethods)]
public readonly partial struct Reps
{
	private static int NormalizeInput(int input) => input;

	private static Validation Validate(int input) =>
		input <= 0
			? Validation.Invalid("Positive count can't be created from number <= 0.")
			: Validation.Ok;
}
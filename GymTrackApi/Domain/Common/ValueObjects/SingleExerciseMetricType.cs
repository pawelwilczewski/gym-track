using Domain.Models.ExerciseInfo;
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject(typeof(ExerciseMetricType))]
public sealed partial class SingleExerciseMetricType
{
	private static ExerciseMetricType NormalizeInput(ExerciseMetricType input) =>

		// todo: normalize (sanitize) your input;
		input;

	private static Validation Validate(ExerciseMetricType input)
	{
		var isValid = true; // todo: your validation
		return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
	}
}
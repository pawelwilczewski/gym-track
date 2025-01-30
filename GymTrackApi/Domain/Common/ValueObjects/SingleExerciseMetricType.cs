using Domain.Models.ExerciseInfo;
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<ExerciseMetricType>]
public sealed partial class SingleExerciseMetricType
{
	private static Validation Validate(ExerciseMetricType input) =>
		input is ExerciseMetricType.Weight or ExerciseMetricType.Distance or ExerciseMetricType.Duration
			? Validation.Ok
			: Validation.Invalid("[todo: describe the validation]");
}
using Domain.Models.ExerciseInfo;
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<ExerciseMetricType>]
public sealed partial class SomeExerciseMetricTypes
{
	private static Validation Validate(ExerciseMetricType input) =>
		input != ExerciseMetricType.None
			? Validation.Ok
			: Validation.Invalid("SomeExerciseMetricTypes can't be set to None.");
}
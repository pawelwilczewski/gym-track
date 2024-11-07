using Domain.Models.Workout;
using Domain.Validation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Common;

internal static class ValidationExtensions
{
	public static ValidationProblem ToValidationProblem(this TextValidationError error, string fieldName) =>
		TypedResults.ValidationProblem(new Dictionary<string, string[]>
		{
			{ fieldName, [error.Error] }
		});

	public static ValidationProblem NotAllowedValidationProblem(this ExerciseMetric metric, string fieldName = "Metric") =>
		TypedResults.ValidationProblem(new Dictionary<string, string[]>
		{
			{ fieldName, [$"Metric type '{metric.Type}' not allowed."] }
		});
}
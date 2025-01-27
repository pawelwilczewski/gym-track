using Domain.Common.Results;
using Domain.Models.ExerciseInfo;
using Microsoft.AspNetCore.Http.HttpResults;
using Vogen;

namespace Api.Common;

internal static class ValidationExtensions
{
	public static ValidationProblem ToValidationProblem(this Validation error, string fieldName) =>
		TypedResults.ValidationProblem(new Dictionary<string, string[]>
		{
			{ fieldName, [error.ErrorMessage] }
		});

	public static ValidationProblem ToValidationProblem(this ValidationError error, string fieldName) =>
		TypedResults.ValidationProblem(new Dictionary<string, string[]>
		{
			{ fieldName, [error.ErrorMessage] }
		});

	public static ValidationProblem NotAllowedValidationProblem(this ExerciseMetric metric, string fieldName = "Metric") =>
		TypedResults.ValidationProblem(new Dictionary<string, string[]>
		{
			{ fieldName, [$"Metric type '{metric.Type}' not allowed."] }
		});
}
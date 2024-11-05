using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Common;

internal static class ValidationErrors
{
	public static ValidationProblem NegativeIndex(string fieldName = "Index") =>
		TypedResults.ValidationProblem(new Dictionary<string, string[]>
		{
			{ fieldName, ["Index must be >= 0."] }
		});
}
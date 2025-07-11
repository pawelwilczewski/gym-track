using System.Reflection;

namespace Application.Tests.Unit;

internal static class ResultExtensions
{
	/// Meant to be used with Result to access Value
	public static object? GetValueOfResult(this object result)
	{
		var unionProperty = result.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance);
		var union = unionProperty?.GetValue(result);

		var valueProperty = union?.GetType().GetProperty("Value", BindingFlags.NonPublic | BindingFlags.Instance);
		return valueProperty?.GetValue(union);
	}
}
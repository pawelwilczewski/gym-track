using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Serialization;

public static class JsonSettings
{
	public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions().Configure();

	public static JsonSerializerOptions Configure(this JsonSerializerOptions options)
	{
		options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

		return options;
	}
}
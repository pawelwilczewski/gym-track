using Microsoft.AspNetCore.Http;

namespace Api.Tests.Mocks;

internal static class RandomGenerator
{
	public static IFormFile FormFile()
	{
		var bytes = "Fake image content"u8.ToArray();
		return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Image", "image.png");
	}
}
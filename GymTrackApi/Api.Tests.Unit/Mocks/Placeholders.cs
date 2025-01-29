using System.Text;
using Domain.Common.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Unit.Mocks;

internal static class Placeholders
{
	public static Name RandomName() => Name.From($"Name_{Guid.NewGuid()}");

	public static Description RandomDescription() => Description.From($"Description_{Guid.NewGuid()}");

	public static FilePath RandomFilePath()
	{
		var pathProvider = new TempFileStoragePathProvider();
		return FilePath.From(Path.Combine(pathProvider.RootPath, $"Files/File_{Guid.NewGuid()}"));
	}

	public static string RandomStringNCharacters(int n = 10)
	{
		var builder = new StringBuilder(n);
		for (var i = 0; i < n; ++i)
		{
			builder.Append(RandomCharacter());
		}

		return builder.ToString();

		char RandomCharacter()
		{
			const int length = 'Z' - 'A';
			return (char)('A' + Random.Shared.Next(length));
		}
	}

	public static IFormFile FormFile()
	{
		var bytes = "Fake image content"u8.ToArray();
		return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Image", "image.png");
	}
}
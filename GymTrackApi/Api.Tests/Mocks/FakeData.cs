using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Mocks;

internal static class FakeData
{
	public static Name RandomName()
	{
		Name.TryCreate($"Name_{Guid.NewGuid()}", out var name, out _);
		return name!;
	}

	public static Description RandomDescription()
	{
		Description.TryCreate($"Description_{Guid.NewGuid()}", out var description, out _);
		return description!;
	}

	public static FilePath RandomFilePath()
	{
		var pathProvider = new TempFileStoragePathProvider();
		FilePath.TryCreate(Path.Combine(pathProvider.RootPath, $"Files/File_{Guid.NewGuid()}"), out var filePath, out _);
		return filePath!;
	}

	public static IFormFile FormFile()
	{
		var bytes = "Fake image content"u8.ToArray();
		return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Image", "image.png");
	}
}
using Domain.Common.ValueObjects;

namespace Application.Files;

internal static class NamedFileExtensions
{
	public static async Task SaveToFile(this NamedFile file, string path, CancellationToken cancellationToken)
	{
		await using var stream = file.Stream;
		var outputStream = File.Create(path);
		await stream.CopyToAsync(outputStream, cancellationToken).ConfigureAwait(false);
		await outputStream.DisposeAsync().ConfigureAwait(false);
	}
}
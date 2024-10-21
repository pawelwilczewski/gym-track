namespace Api.Files;

internal static class FormFileExtensions
{
	public static async Task SaveToFile(this IFormFile file, string path, CancellationToken cancellationToken)
	{
		var stream = file.OpenReadStream();
		await using (stream.ConfigureAwait(false))
		{
			var outputStream = File.Create(path);
			await stream.CopyToAsync(outputStream, cancellationToken).ConfigureAwait(false);
			await outputStream.DisposeAsync().ConfigureAwait(false);
		}
	}
}
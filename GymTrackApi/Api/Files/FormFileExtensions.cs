using Domain.Common.ValueObjects;

namespace Api.Files;

internal static class FormFileExtensions
{
	public static NamedFile AsNamedFile(this IFormFile file) =>
		new(file.FileName, file.OpenReadStream());
}
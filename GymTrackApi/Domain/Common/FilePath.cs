using System.ComponentModel.DataAnnotations;

namespace Domain.Common;

public readonly record struct FilePath(
	[property: MaxLength(FilePath.MAX_LENGTH, ErrorMessage = FilePath.TOO_LONG_ERROR)]
	string Path)
{
	internal const int MAX_LENGTH = 500;
	internal const string TOO_LONG_ERROR = "File path too long.";
}

public readonly record struct OptionalFilePath(
	[property: MaxLength(FilePath.MAX_LENGTH, ErrorMessage = FilePath.TOO_LONG_ERROR)]
	string? Path);
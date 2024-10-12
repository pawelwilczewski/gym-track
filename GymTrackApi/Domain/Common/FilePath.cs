namespace Domain.Common;

public readonly record struct FilePath(string Path);

public readonly record struct OptionalFilePath(string? Path);
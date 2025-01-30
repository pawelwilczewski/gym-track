namespace Domain.Common.ValueObjects;

public readonly record struct NamedFile(string FileName, Stream Stream);
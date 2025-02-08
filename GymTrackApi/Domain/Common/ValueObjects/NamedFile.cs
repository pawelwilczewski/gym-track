namespace Domain.Common.ValueObjects;

// TODO Pawel: we could have a FileName VO here as well that validates file name characters/extension etc.
public readonly record struct NamedFile(string FileName, Stream Stream);
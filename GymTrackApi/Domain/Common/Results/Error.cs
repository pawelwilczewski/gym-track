namespace Domain.Common.Results;

public readonly record struct Error
{
	public static Error Instance { get; } = new();
}
namespace Domain.Common.Results;

public readonly record struct Success
{
	public static Success Instance { get; } = new();
}

public readonly record struct Success<TValue>(TValue Value);
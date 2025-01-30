using Domain.Common.ValidationExtensions;
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct Description
{
	public const int MAX_LENGTH = 2000;

	private static Validation Validate(string input) =>
		!input.IsNotNull(out var error)
		|| !input.HasMaxLength(MAX_LENGTH, out error)
			? Validation.Invalid(error.Value.ErrorMessage)
			: Validation.Ok;

	private static string NormalizeInput(string input) => input.Trim();
}
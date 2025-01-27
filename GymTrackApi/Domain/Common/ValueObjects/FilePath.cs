using Domain.Common.ValidationExtensions;
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject(typeof(string))]
public readonly partial struct FilePath
{
	public const int MAX_LENGTH = 500;

	private static Validation Validate(string input) =>
		!input.IsNotNullOrWhitespace(out var error)
		|| !input.IsNotOnlyPunctuation(out error)
		|| !input.HasMaxLength(MAX_LENGTH, out error)
		|| !input.HasMinLength(1, out error)
		|| !input.HasNoInvalidPathCharacters(out error)
			? Validation.Invalid(error.Value.ErrorMessage)
			: Validation.Ok;

	private static string NormalizeInput(string input) => input.Trim();
}
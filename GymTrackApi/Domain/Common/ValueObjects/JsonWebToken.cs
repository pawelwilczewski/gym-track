using Domain.Common.ValidationExtensions;
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct JsonWebToken
{
	private static Validation Validate(string input) =>
		!input.IsNotNullOrWhitespace(out var error)
			? Validation.Invalid(error.Value.ErrorMessage)
			: Validation.Ok;
}
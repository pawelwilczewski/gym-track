using System.ComponentModel.DataAnnotations;
using Domain.Common.ValidationExtensions;
using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct EmailAddress
{
	public const int MAX_LENGTH = 254;

	private static readonly EmailAddressAttribute checker = new();

	private static Validation Validate(string input) =>
		input.IsNotNullOrWhitespace(out _) && input.Length <= MAX_LENGTH && checker.IsValid(input)
			? Validation.Ok
			: Validation.Invalid("Invalid email address.");
}
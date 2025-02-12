using Vogen;

namespace Domain.Common.ValueObjects;

[ValueObject<string>]
public readonly partial struct EmailConfirmationCode
{
	public const int BYTES_LENGTH = 16;
}
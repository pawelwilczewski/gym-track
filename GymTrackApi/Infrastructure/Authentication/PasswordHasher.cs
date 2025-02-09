using System.Security.Cryptography;
using Application.Auth.Abstractions;
using Domain.Common.ValueObjects;

namespace Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
	private const int ITERATIONS = 500000;

	private static readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA512;

	public PasswordHash Hash(Password password)
	{
		var salt = RandomNumberGenerator.GetBytes(PasswordHash.SALT_BYTES_LENGTH);
		return Hash(password, PasswordSalt.From(Convert.ToHexString(salt)));
	}

	public PasswordHash Hash(Password password, PasswordSalt salt)
	{
		var hash = Rfc2898DeriveBytes.Pbkdf2(
			password.Value,
			Convert.FromHexString(salt.Value),
			ITERATIONS,
			algorithm,
			PasswordHash.HASH_BYTES_LENGTH);

		return PasswordHash.From($"{salt}{PasswordHash.DELIMITER}{Convert.ToHexString(hash)}");
	}
}
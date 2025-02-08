using Application.Auth.Abstractions;
using Domain.Common.ValueObjects;

namespace Infrastructure.Authentication;

internal sealed class PasswordVerifier : IPasswordVerifier
{
	private readonly IPasswordHasher passwordHasher;

	public PasswordVerifier(IPasswordHasher passwordHasher) =>
		this.passwordHasher = passwordHasher;

	public bool Verify(Password password, PasswordHash passwordHash) =>
		passwordHash.Value == passwordHasher.Hash(password, passwordHash.Salt).Value;
}
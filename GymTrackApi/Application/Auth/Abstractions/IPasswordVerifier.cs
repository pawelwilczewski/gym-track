using Domain.Common.ValueObjects;

namespace Application.Auth.Abstractions;

public interface IPasswordVerifier
{
	bool Verify(Password password, PasswordHash passwordHash);
}
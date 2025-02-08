using Domain.Common.ValueObjects;

namespace Application.Auth.Abstractions;

public interface IPasswordHasher
{
	PasswordHash Hash(Password password);
	PasswordHash Hash(Password password, PasswordSalt salt);
}
using Domain.Models.User;

namespace Application.Auth.Abstractions;

public interface IPasswordResetCodeGenerator
{
	PasswordResetCodeData Generate();
}
using Domain.Models.User;

namespace Application.Auth.Abstractions;

public interface IEmailConfirmationCodeGenerator
{
	EmailConfirmationCodeData Generate();
}
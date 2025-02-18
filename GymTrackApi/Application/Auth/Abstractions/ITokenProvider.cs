using Domain.Common.ValueObjects;
using Domain.Models.User;

namespace Application.Auth.Abstractions;

public interface ITokenProvider
{
	JsonWebToken Create(User user, string audience);
}
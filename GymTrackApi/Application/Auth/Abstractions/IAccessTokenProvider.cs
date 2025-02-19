using Domain.Common.ValueObjects;
using Domain.Models.User;

namespace Application.Auth.Abstractions;

public interface IAccessTokenProvider
{
	JsonWebToken Create(User user, string requestingAudience);
}
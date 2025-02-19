using Domain.Models.User;

namespace Application.Auth.Abstractions;

public interface IRefreshTokenProvider
{
	RefreshTokenData Create();
}
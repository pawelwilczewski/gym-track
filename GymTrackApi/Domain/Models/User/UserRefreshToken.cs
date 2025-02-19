// ReSharper disable UnusedAutoPropertyAccessor.Local

using Domain.Common.ValueObjects;
using Vogen;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.User;

public class UserRefreshToken
{
	public UserRefreshTokenId Id { get; private set; } = UserRefreshTokenId.New();

	public UserId UserId { get; private set; }

	public User User { get; private set; }

	public RefreshToken RefreshToken { get; private set; }

	public RefreshTokenExpiryDateTime ExpiresAt { get; private set; }

	public RefreshTokenData Data => new(RefreshToken, ExpiresAt);

	private UserRefreshToken() { }

	private UserRefreshToken(User user, RefreshTokenData data)
	{
		User = user;
		UserId = user.Id;
		RefreshToken = data.Token;
		ExpiresAt = data.ExpiresAt;
	}

	public static UserRefreshToken Create(User user, RefreshTokenData data) => new(user, data);

	public bool IsTokenValid(RefreshToken token) =>
		DateTime.UtcNow < ExpiresAt.Value && token == RefreshToken;
}

public sealed record class RefreshTokenData(RefreshToken Token, RefreshTokenExpiryDateTime ExpiresAt);

[ValueObject<Guid>]
public readonly partial struct UserRefreshTokenId
{
	public static UserRefreshTokenId New() => From(Ulid.NewUlid().ToGuid());
}
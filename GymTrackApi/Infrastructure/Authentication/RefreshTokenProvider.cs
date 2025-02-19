using System.Security.Cryptography;
using Application.Auth.Abstractions;
using Application.Settings;
using Domain.Common.ValueObjects;
using Domain.Models.User;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

internal sealed class RefreshTokenProvider : IRefreshTokenProvider
{
	private readonly RefreshTokenSettings settings;

	public RefreshTokenProvider(IOptions<RefreshTokenSettings> settings) => this.settings = settings.Value;

	public RefreshTokenData Create()
	{
		var code = Convert.ToHexString(RandomNumberGenerator.GetBytes(RefreshToken.BYTES_LENGTH));
		return new RefreshTokenData(
			RefreshToken.From(code),
			RefreshTokenExpiryDateTime.From(
				DateTime.UtcNow + TimeSpan.FromMinutes(settings.ExpiryTimeInMinutes)));
	}
}
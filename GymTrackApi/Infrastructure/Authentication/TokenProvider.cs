using System.Security.Claims;
using System.Text;
using Application.Auth.Abstractions;
using Domain.Models.User;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JsonWebToken = Domain.Common.ValueObjects.JsonWebToken;

namespace Infrastructure.Authentication;

internal sealed class TokenProvider : ITokenProvider
{
	private static readonly JsonWebTokenHandler jwtHandler = new();
	private readonly JwtSettings jwtSettings;
	private readonly FrontendSettings frontendSettings;

	public TokenProvider(IOptions<JwtSettings> jwtSettings, IOptions<FrontendSettings> frontendSettings)
	{
		this.jwtSettings = jwtSettings.Value;
		this.frontendSettings = frontendSettings.Value;
	}

	public JsonWebToken Create(User user)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		return JsonWebToken.From(jwtHandler.CreateToken(new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(
			[
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email.Value)
			]),
			Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationInMinutes),
			SigningCredentials = credentials,
			Issuer = jwtSettings.Issuer,
			Audience = frontendSettings.BaseUrl
		}));
	}
}
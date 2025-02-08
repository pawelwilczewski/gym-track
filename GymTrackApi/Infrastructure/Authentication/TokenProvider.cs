using System.Security.Claims;
using System.Text;
using Application.Auth.Abstractions;
using Domain.Models.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JsonWebToken = Domain.Common.ValueObjects.JsonWebToken;

namespace Infrastructure.Authentication;

internal sealed class TokenProvider : ITokenProvider
{
	private static readonly JsonWebTokenHandler jwtHandler = new();
	private readonly IConfiguration configuration;

	public TokenProvider(IConfiguration configuration) =>
		this.configuration = configuration;

	public JsonWebToken Create(User user)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		return JsonWebToken.From(jwtHandler.CreateToken(new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(
			[
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email.Value)
			]),
			Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
			SigningCredentials = credentials,
			Issuer = configuration["Jwt:Issuer"],
			Audience = configuration["Jwt:Audience"]
		}));
	}
}
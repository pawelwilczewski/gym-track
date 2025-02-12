using System.Security.Cryptography;
using Application.Auth.Abstractions;
using Domain.Common.ValueObjects;
using Domain.Models.User;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

internal sealed class PasswordResetCodeGenerator : IPasswordResetCodeGenerator
{
	private readonly PasswordResetSettings settings;

	public PasswordResetCodeGenerator(IOptions<PasswordResetSettings> settings) => this.settings = settings.Value;

	public PasswordResetCodeData Generate()
	{
		var code = Convert.ToHexString(RandomNumberGenerator.GetBytes(PasswordResetCode.BYTES_LENGTH));
		return new PasswordResetCodeData(
			PasswordResetCode.From(code),
			PasswordResetCodeExpiryDateTime.From(
				DateTime.UtcNow + TimeSpan.FromMinutes(settings.ExpiryTimeInMinutes)));
	}
}
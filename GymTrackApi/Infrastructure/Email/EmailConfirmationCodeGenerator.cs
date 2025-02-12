using System.Security.Cryptography;
using Application.Auth.Abstractions;
using Domain.Common.ValueObjects;
using Domain.Models.User;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email;

internal sealed class EmailConfirmationCodeGenerator : IEmailConfirmationCodeGenerator
{
	private readonly EmailConfirmationSettings settings;

	public EmailConfirmationCodeGenerator(IOptions<EmailConfirmationSettings> settings) => this.settings = settings.Value;

	public EmailConfirmationCodeData Generate()
	{
		var code = Convert.ToHexString(RandomNumberGenerator.GetBytes(EmailConfirmationCode.BYTES_LENGTH));
		return new EmailConfirmationCodeData(
			EmailConfirmationCode.From(code),
			EmailConfirmationCodeExpiryDateTime.From(
				DateTime.UtcNow + TimeSpan.FromMinutes(settings.ExpiryTimeInMinutes)));
	}
}
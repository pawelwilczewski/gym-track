using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Application.Email;
using Domain.Common.ValueObjects;
using Domain.Models.User;

namespace Api.Tests.Functional;

internal sealed class FakeUserEmailSenderCache : IUserEmailSender
{
	private const double GET_TIMEOUT_SECONDS = 30;
	private const double GET_CHECK_INTERVAL_SECONDS = 1;

	private static readonly ConcurrentDictionary<EmailAddress, EmailConfirmationCode> emailConfirmationCodes = [];
	private static readonly ConcurrentDictionary<EmailAddress, PasswordResetCode> passwordResetCodes = [];

	public Task SendEmailConfirmationLink(User user, EmailConfirmationCodeData data, CancellationToken cancellationToken)
	{
		emailConfirmationCodes[user.Email] = data.Code;
		return Task.CompletedTask;
	}

	public Task SendPasswordResetLink(User user, PasswordResetCodeData data, CancellationToken cancellationToken)
	{
		passwordResetCodes[user.Email] = data.Code;
		return Task.CompletedTask;
	}

	public static Task<EmailConfirmationCode> GetEmailConfirmationCode(EmailAddress email) =>
		TryGetValueWithinTimeout(email, (out EmailConfirmationCode code) =>
			emailConfirmationCodes.TryGetValue(email, out code));

	public static void ClearEmailConfirmationCode(EmailAddress email) => emailConfirmationCodes.Remove(email, out _);

	public static Task<PasswordResetCode> GetPasswordResetCode(EmailAddress email) =>
		TryGetValueWithinTimeout(email, (out PasswordResetCode code) =>
			passwordResetCodes.TryGetValue(email, out code));

	public static void ClearPasswordResetCode(EmailAddress email) => passwordResetCodes.Remove(email, out _);

	private static async Task<TValue> TryGetValueWithinTimeout<TValue>(EmailAddress email, TryGetValue<TValue> tryGetValue)
	{
		var count = (int)Math.Ceiling(GET_TIMEOUT_SECONDS / GET_CHECK_INTERVAL_SECONDS) + 1;
		for (var i = 0; i < count; i++)
		{
			if (tryGetValue(out var value)) return value;

			await Task.Delay(TimeSpan.FromSeconds(GET_CHECK_INTERVAL_SECONDS)).ConfigureAwait(false);
		}

		throw new Exception("No email confirmation code found within timeout limit");
	}

	private delegate bool TryGetValue<T>([NotNullWhen(true)] out T? value);
}
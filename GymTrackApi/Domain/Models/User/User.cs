using Domain.Common;
using Domain.Common.Collections;
using Domain.Common.ValueObjects;
using Domain.Models.Tracking;
using Vogen;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.User;

public class User : AggregateRoot
{
	public UserId Id { get; private set; } = UserId.New();

	public EmailAddress Email { get; private set; }

	public bool HasConfirmedEmail { get; private set; }

	public PasswordHash PasswordHash { get; private set; }

	public IReadOnlyList<UserEmailConfirmationCode> EmailConfirmationCodes => emailConfirmationCodes.AsReadOnly();
	private readonly ListWithMaxCapacity<UserEmailConfirmationCode> emailConfirmationCodes = new(3, RemoveItemStrategies.RemoveSoonestExpiring);

	public virtual IReadOnlyList<UserPasswordResetCode> PasswordResetCodes => passwordResetCodes.AsReadOnly();
	private readonly ListWithMaxCapacity<UserPasswordResetCode> passwordResetCodes = new(3, RemoveItemStrategies.RemoveSoonestExpiring);

	public IReadOnlyList<UserRefreshToken> RefreshTokens => refreshTokens.AsReadOnly();
	private readonly List<UserRefreshToken> refreshTokens = [];

	public virtual List<Workout.Workout> Workouts { get; private set; } = [];
	public virtual List<ExerciseInfo.ExerciseInfo> ExerciseInfos { get; private set; } = [];
	public virtual List<TrackedWorkout> TrackedWorkouts { get; private set; } = [];

	private User() { }

	private User(EmailAddress email, PasswordHash passwordHash)
	{
		Email = email;
		HasConfirmedEmail = false;
		PasswordHash = passwordHash;
	}

	public static User Create(EmailAddress email, PasswordHash passwordHash)
	{
		var user = new User(email, passwordHash);
		user.Raise(new UserCreatedEvent
		{
			UserId = user.Id
		});
		return user;
	}

	public void AddEmailConfirmationCode(EmailConfirmationCodeData data) =>
		emailConfirmationCodes.Add(UserEmailConfirmationCode.Create(this, data));

	public void InvalidateEmailConfirmationCodes() => emailConfirmationCodes.Clear();

	public bool TryConfirmEmail(EmailConfirmationCode emailConfirmationCode)
	{
		if (HasConfirmedEmail || EmailConfirmationCodes.Any(code => code.IsCodeValid(emailConfirmationCode)))
		{
			HasConfirmedEmail = true;
			InvalidateEmailConfirmationCodes();
			return true;
		}

		return false;
	}

	public void AddPasswordResetCode(PasswordResetCodeData data) =>
		passwordResetCodes.Add(UserPasswordResetCode.Create(this, data));

	public void InvalidatePasswordResetCodes() => passwordResetCodes.Clear();

	public void UpdatePasswordHash(PasswordHash passwordHash)
	{
		PasswordHash = passwordHash;
		InvalidatePasswordResetCodes();
	}

	public void AddRefreshTokenAndCleanUp(RefreshTokenData refreshTokenData)
	{
		refreshTokens.Add(UserRefreshToken.Create(this, refreshTokenData));
		refreshTokens.RemoveAll(refreshToken => refreshToken.ExpiresAt.Value < DateTime.Now);
	}

	public void RemoveRefreshToken(RefreshToken refreshToken) =>
		refreshTokens.RemoveAll(token => token.RefreshToken == refreshToken);

	public void InvalidateRefreshTokens() => refreshTokens.Clear();
}

[ValueObject<Guid>]
public readonly partial struct UserId
{
	public static UserId New() => From(Ulid.NewUlid().ToGuid());
}
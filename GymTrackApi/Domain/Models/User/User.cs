using Domain.Common;
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

	public virtual UserEmailConfirmationCode? EmailConfirmationCode { get; private set; }

	public virtual UserPasswordResetCode? PasswordResetCode { get; private set; }

	public virtual UserRefreshToken? RefreshToken { get; private set; }

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

	public void UpdateEmailConfirmationCode(EmailConfirmationCodeData data) =>
		EmailConfirmationCode = UserEmailConfirmationCode.Create(this, data);

	public void DeleteEmailConfirmationCode() => EmailConfirmationCode = null;

	public bool TryConfirmEmail(EmailConfirmationCode emailConfirmationCode)
	{
		if (HasConfirmedEmail) return true;

		if (EmailConfirmationCode is null) return false;

		if (EmailConfirmationCode.IsCodeValid(emailConfirmationCode))
		{
			HasConfirmedEmail = true;
			return true;
		}

		return false;
	}

	public void UpdatePasswordHash(PasswordHash passwordHash) => PasswordHash = passwordHash;

	public void UpdatePasswordResetCode(PasswordResetCodeData data) =>
		PasswordResetCode = UserPasswordResetCode.Create(this, data);

	public void DeletePasswordResetCode() => PasswordResetCode = null;

	public void UpdateRefreshToken(RefreshTokenData refreshTokenData) =>
		RefreshToken = UserRefreshToken.Create(this, refreshTokenData);
}

[ValueObject<Guid>]
public readonly partial struct UserId
{
	public static UserId New() => From(Ulid.NewUlid().ToGuid());
}
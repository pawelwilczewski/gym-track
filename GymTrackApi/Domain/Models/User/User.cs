using System.Diagnostics.CodeAnalysis;
using Domain.Common;
using Domain.Common.Results;
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

	public bool TryUpdateEmailConfirmationCode(
		EmailConfirmationCodeData data,
		[NotNullWhen(false)] out ValidationError? error)
	{
		if (!UserEmailConfirmationCode.TryCreate(this, data, out var confirmationCode, out error))
		{
			return false;
		}

		EmailConfirmationCode = confirmationCode;
		return true;
	}

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
}

[ValueObject<Guid>]
public readonly partial struct UserId
{
	public static UserId New() => From(Ulid.NewUlid().ToGuid());
}
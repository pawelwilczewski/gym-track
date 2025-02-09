using Domain.Common;
using Domain.Common.ValueObjects;
using Domain.Models.Tracking;
using Vogen;

namespace Domain.Models.User;

public class User : AggregateRoot
{
	public UserId Id { get; } = UserId.New();

	public EmailAddress Email { get; private set; }

	public bool HasConfirmedEmail { get; private set; }

	public PasswordHash PasswordHash { get; private set; }

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

	public void ConfirmEmail() => HasConfirmedEmail = true;
}

[ValueObject<Guid>]
public readonly partial struct UserId
{
	public static UserId New() => From(Ulid.NewUlid().ToGuid());
}
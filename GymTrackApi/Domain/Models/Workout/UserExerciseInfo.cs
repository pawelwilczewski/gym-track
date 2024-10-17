using Domain.Models.Identity;

namespace Domain.Models.Workout;

public class UserExerciseInfo
{
	public Guid UserId { get; private set; }
	public Id<ExerciseInfo> ExerciseInfoId { get; private set; }

	public virtual User User { get; private set; } = default!;
	public virtual ExerciseInfo ExerciseInfo { get; private set; } = default!;

	// ReSharper disable once UnusedMember.Local
	private UserExerciseInfo() { }

	public UserExerciseInfo(Guid userId, Id<ExerciseInfo> exerciseInfoId)
	{
		UserId = userId;
		ExerciseInfoId = exerciseInfoId;
	}
}
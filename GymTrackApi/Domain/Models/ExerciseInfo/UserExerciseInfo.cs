using Domain.Models.Common;
using Domain.Models.Identity;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Domain.Models.ExerciseInfo;

public class UserExerciseInfo : IUserOwned
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
using Domain.Models.Identity;

namespace Domain.Models.Workout;

public class UserExerciseInfo
{
	public Guid UserId { get; set; }
	public Id<ExerciseInfo> ExerciseInfoId { get; set; }

	public virtual required User User { get; set; }
	public virtual required ExerciseInfo ExerciseInfo { get; set; }
}
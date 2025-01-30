using Domain.Models.Tracking;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Identity;

public class User : IdentityUser<Guid>
{
	public virtual List<Workout.Workout> Workouts { get; private set; } = [];
	public virtual List<ExerciseInfo.ExerciseInfo> ExerciseInfos { get; private set; } = [];
	public virtual List<TrackedWorkout> TrackedWorkouts { get; private set; } = [];
}
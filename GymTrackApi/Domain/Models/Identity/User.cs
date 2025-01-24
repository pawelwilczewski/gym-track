using Domain.Models.ExerciseInfo;
using Domain.Models.Tracking;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Identity;

public class User : IdentityUser<Guid>
{
	public virtual List<UserWorkout> Workouts { get; set; } = [];
	public virtual List<UserExerciseInfo> ExerciseInfos { get; set; } = [];
	public virtual List<TrackedWorkout> TrackedWorkouts { get; set; } = [];
}
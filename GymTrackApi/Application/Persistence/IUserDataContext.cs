using Domain.Models.Identity;
using Domain.Models.Tracking;

namespace Application.Persistence;

public interface IUserDataContext
{
	IUserAssetRepository<Domain.Models.ExerciseInfo.ExerciseInfo> ExerciseInfos { get; }
	IUserAssetRepository<Domain.Models.Workout.Workout> Workouts { get; }
	IUserAssetRepository<TrackedWorkout> TrackedWorkouts { get; }

	Task<User> GetUser(CancellationToken cancellationToken);

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
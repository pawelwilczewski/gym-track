using Domain.Models.Tracking;
using Domain.Models.User;

namespace Application.Persistence;

public interface IUserDataContext
{
	IUserAssetRepository<Domain.Models.ExerciseInfo.ExerciseInfo> ExerciseInfos { get; }
	IUserAssetRepository<Domain.Models.Workout.Workout> Workouts { get; }
	IUserAssetRepository<TrackedWorkout> TrackedWorkouts { get; }

	Task<User> GetUser(CancellationToken cancellationToken);

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
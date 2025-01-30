using Application.Persistence;
using Domain.Models.ExerciseInfo;
using Domain.Models.Identity;
using Domain.Models.Tracking;
using Domain.Models.Workout;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class UserDataContext : IUserDataContext
{
	public IUserAssetRepository<ExerciseInfo> ExerciseInfos { get; }
	public IUserAssetRepository<Workout> Workouts { get; }
	public IUserAssetRepository<TrackedWorkout> TrackedWorkouts { get; }

	private readonly Guid userId;
	private readonly AppDbContext dbContext;

	public UserDataContext(Guid userId, AppDbContext dbContext)
	{
		this.userId = userId;
		this.dbContext = dbContext;

		ExerciseInfos = new UserAssetRepository<ExerciseInfo>(userId, this.dbContext);
		Workouts = new UserAssetRepository<Workout>(userId, this.dbContext);
		TrackedWorkouts = new UserAssetRepository<TrackedWorkout>(userId, this.dbContext);
	}

	public Task<User> GetUser(CancellationToken cancellationToken) =>
		dbContext.Users.FirstAsync(user => user.Id == userId, cancellationToken);

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
		dbContext.SaveChangesAsync(cancellationToken);
}
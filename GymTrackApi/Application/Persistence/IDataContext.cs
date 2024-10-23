using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.EntityFrameworkCore;

namespace Application.Persistence;

public interface IDataContext : IDisposable
{
	DbSet<User> Users { get; }
	DbSet<Role> Roles { get; }
	DbSet<Workout> Workouts { get; }
	DbSet<UserWorkout> UserWorkouts { get; }
	DbSet<ExerciseInfo> ExerciseInfos { get; }
	DbSet<ExerciseInfo.Step> ExerciseInfoSteps { get; }
	DbSet<Exercise> Exercises { get; }
	DbSet<ExerciseSet> ExerciseSets { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
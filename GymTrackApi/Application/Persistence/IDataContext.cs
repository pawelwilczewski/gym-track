using Domain.Models.ExerciseInfo;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.EntityFrameworkCore;

namespace Application.Persistence;

public interface IDataContext : IDisposable
{
	DbSet<User> Users { get; }
	DbSet<Role> Roles { get; }

	DbSet<UserExerciseInfo> UserExerciseInfos { get; }
	DbSet<ExerciseInfo> ExerciseInfos { get; }
	DbSet<ExerciseInfo.Step> ExerciseInfoSteps { get; }

	DbSet<UserWorkout> UserWorkouts { get; }
	DbSet<Workout> Workouts { get; }
	DbSet<Workout.Exercise> WorkoutExercises { get; }
	DbSet<Workout.Exercise.Set> WorkoutExerciseSets { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
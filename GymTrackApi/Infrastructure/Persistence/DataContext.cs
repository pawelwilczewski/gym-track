using Application.Persistence;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class DataContext : IDataContext
{
	public DbSet<User> Users => dbContext.Users;
	public DbSet<Role> Roles => dbContext.Roles;
	public DbSet<Workout> Workouts => dbContext.Workouts;
	public DbSet<UserWorkout> UserWorkouts => dbContext.UserWorkouts;
	public DbSet<ExerciseInfo> ExerciseInfos => dbContext.ExerciseInfos;
	public DbSet<ExerciseInfo.Step> ExerciseInfoSteps => dbContext.ExerciseInfoSteps;
	public DbSet<Exercise> Exercises => dbContext.Exercises;
	public DbSet<ExerciseSet> ExerciseSets => dbContext.ExerciseSets;

	private readonly AppDbContext dbContext;

	public DataContext(AppDbContext dbContext) => this.dbContext = dbContext;

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
		dbContext.SaveChangesAsync(cancellationToken);

	public void Dispose() => dbContext.Dispose();
}
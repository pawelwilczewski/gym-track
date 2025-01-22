using Application.Persistence;
using Domain.Models.ExerciseInfo;
using Domain.Models.Identity;
using Domain.Models.Tracking;
using Domain.Models.Workout;
using Infrastructure.Persistence.Configurations.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Infrastructure.Persistence;

internal sealed class AppDbContext : IdentityDbContext<User, Role, Guid>, IDataContext
{
	public DbSet<UserWorkout> UserWorkouts { get; private set; } = null!;
	public DbSet<Workout> Workouts { get; private set; } = null!;
	public DbSet<Workout.Exercise> WorkoutExercises { get; private set; } = null!;
	public DbSet<Workout.Exercise.Set> WorkoutExerciseSets { get; private set; } = null!;

	public DbSet<UserExerciseInfo> UserExerciseInfos { get; private set; } = null!;
	public DbSet<ExerciseInfo> ExerciseInfos { get; private set; } = null!;
	public DbSet<ExerciseInfo.Step> ExerciseInfoSteps { get; private set; } = null!;

	public DbSet<TrackedWorkout> TrackedWorkouts { get; private set; } = null!;

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) =>
		ChangeTracker.LazyLoadingEnabled = false;

	public AppDbContext() // for creating migrations
		: this(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql().Options) { }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

		builder.HasDefaultSchema(Schemas.IDENTITY);

		builder.RegisterConfigurationsInAssembly();
	}
}
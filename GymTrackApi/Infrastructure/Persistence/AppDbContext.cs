using Domain.Models.ExerciseInfo;
using Domain.Models.Tracking;
using Domain.Models.User;
using Domain.Models.Workout;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Configurations.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class AppDbContext : DbContext
{
	public DbSet<User> Users { get; private set; } = null!;

	public DbSet<Workout> Workouts { get; private set; } = null!;
	public DbSet<WorkoutExercise> WorkoutExercises { get; private set; } = null!;
	public DbSet<WorkoutExerciseSet> WorkoutExerciseSets { get; private set; } = null!;

	public DbSet<ExerciseInfo> ExerciseInfos { get; private set; } = null!;
	public DbSet<ExerciseInfoStep> ExerciseInfoSteps { get; private set; } = null!;

	public DbSet<TrackedWorkout> TrackedWorkouts { get; private set; } = null!;

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) =>
		ChangeTracker.LazyLoadingEnabled = false;

	public AppDbContext() // for creating migrations
		: this(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql().Options) { }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

		builder.RegisterConfigurationsInAssembly();

		builder.AddInboxStateEntity(inboxBuilder =>
			inboxBuilder.ToTable("Inbox", Schemas.INFRASTRUCTURE));

		builder.AddOutboxStateEntity(outboxBuilder =>
			outboxBuilder.ToTable("Outbox", Schemas.INFRASTRUCTURE));

		builder.AddOutboxMessageEntity(messageBuilder =>
			messageBuilder.ToTable("Messages", Schemas.INFRASTRUCTURE));
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
		configurationBuilder.ConfigureProperties();
}
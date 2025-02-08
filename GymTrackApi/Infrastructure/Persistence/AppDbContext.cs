using Domain.Common;
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

	private readonly IPublishEndpoint publishEndpoint;

	public AppDbContext(DbContextOptions<AppDbContext> options, IPublishEndpoint publishEndpoint) : base(options)
	{
		this.publishEndpoint = publishEndpoint;

		ChangeTracker.LazyLoadingEnabled = false;
	}

	public AppDbContext() // for creating migrations
		: this(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql().Options, null!) { }

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

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		var domainEvents = ChangeTracker.Entries<AggregateRoot>()
			.Select(entry => entry.Entity)
			.SelectMany(root =>
			{
				var events = root.DomainEvents.ToList();
				root.ClearDomainEvents();
				return events;
			});

		foreach (var @event in domainEvents)
		{
			await publishEndpoint.Publish(@event, cancellationToken).ConfigureAwait(false);
		}

		var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

		return result;
	}
}
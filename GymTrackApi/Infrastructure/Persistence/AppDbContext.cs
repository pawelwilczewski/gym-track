using System.Reflection;
using Domain.Models.Identity;
using Domain.Models.Workout;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class AppDbContext : IdentityDbContext<User, Role, Guid>
{
	public DbSet<Workout> Workouts { get; private set; } = null!;
	public DbSet<UserWorkout> UserWorkouts { get; private set; } = null!;
	public DbSet<ExerciseInfo> ExerciseInfos { get; private set; } = null!;
	public DbSet<ExerciseStepInfo> ExerciseStepInfos { get; private set; } = null!;
	public DbSet<Exercise> Exercises { get; private set; } = null!;
	public DbSet<ExerciseSet> ExerciseSets { get; private set; } = null!;

	public AppDbContext() // for creating migrations
		: base(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql().Options) { }

	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) { }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		ChangeTracker.LazyLoadingEnabled = false;
		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

		builder.HasDefaultSchema(Schemas.IDENTITY);

		builder.RegisterConfigurationsInAssembly();
	}
}

public static class ModelBuilderExtensions
{
	public static void RegisterConfigurationsInAssembly(this ModelBuilder builder)
	{
		var typesToRegister = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(type => type.GetInterfaces()
				.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

		foreach (var type in typesToRegister)
		{
			var entityType = type.GetInterfaces()
				.First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
				.GetGenericArguments()[0];

			var applyConfigMethod = typeof(ModelBuilder)
				.GetMethod(nameof(ModelBuilder.ApplyConfiguration))
				?.MakeGenericMethod(entityType);

			var configurationInstance = Activator.CreateInstance(type);

			applyConfigMethod?.Invoke(builder, [configurationInstance]);
		}
	}
}
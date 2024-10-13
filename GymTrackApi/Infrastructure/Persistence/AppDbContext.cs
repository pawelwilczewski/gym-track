using System.Reflection;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class AppDbContext : IdentityDbContext<User, Role, Guid>
{
	public AppDbContext() // for creating migrations
		: base(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql().Options) { }

	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) { }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.HasDefaultSchema(Schemas.IDENTITY);

		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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
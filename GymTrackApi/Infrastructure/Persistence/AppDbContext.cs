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
			.Where(type => type.BaseType is { IsGenericType: true }
				&& type.BaseType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));

		foreach (var type in typesToRegister)
		{
			dynamic configurationInstance = Activator.CreateInstance(type)!;
			builder.ApplyConfiguration(configurationInstance);
		}
	}
}
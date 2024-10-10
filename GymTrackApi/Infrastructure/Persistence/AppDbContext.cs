using Domain.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;

internal sealed class AppDbContext : IdentityDbContext<AppUser, Role, Guid>
{
	public AppDbContext() // for creating migrations
		: base(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql().Options) { }

	public AppDbContext(IConfiguration configuration) // nullable for creating migrations
		: base(GetOptions(configuration)) { }

	private static DbContextOptions<AppDbContext> GetOptions(IConfiguration configuration) =>
		new DbContextOptionsBuilder<AppDbContext>()
			.UseNpgsql(configuration.GetConnectionString("AppDb"))
			.EnableSensitiveDataLogging()
			.Options;

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.HasDefaultSchema("Identity");

		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

		builder.Entity<AppUser>()
			.Property(u => u.Id)
			.HasDefaultValueSql("uuid_generate_v4()");

		builder.Entity<Role>()
			.Property(u => u.Id)
			.HasDefaultValueSql("uuid_generate_v4()");
	}
}
using Domain.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;

internal sealed class AppDbContext(IConfiguration configuration)
	: IdentityDbContext<AppUser, Role, Guid>(GetOptions(configuration))
{
	private static DbContextOptions<AppDbContext> GetOptions(IConfiguration configuration) =>
		new DbContextOptionsBuilder<AppDbContext>()
			.UseNpgsql(configuration.GetConnectionString("AppDb"))
			.EnableSensitiveDataLogging()
			.Options;

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.HasDefaultSchema("Identity");
	}
}
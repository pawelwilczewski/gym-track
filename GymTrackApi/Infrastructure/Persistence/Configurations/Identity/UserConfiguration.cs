using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Identity;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder
			.Property(u => u.Id)
			.HasDefaultValueSql("uuid_generate_v4()");

		builder
			.HasMany(user => user.Workouts)
			.WithOne()
			.HasForeignKey("ownerId")
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(user => user.ExerciseInfos)
			.WithOne()
			.HasForeignKey("ownerId")
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(user => user.TrackedWorkouts)
			.WithOne()
			.HasForeignKey("ownerId")
			.OnDelete(DeleteBehavior.Cascade);
	}
}
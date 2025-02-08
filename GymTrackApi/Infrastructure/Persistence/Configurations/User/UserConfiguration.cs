using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.User;

internal sealed class UserConfiguration : IEntityTypeConfiguration<Domain.Models.User.User>
{
	public void Configure(EntityTypeBuilder<Domain.Models.User.User> builder)
	{
		builder
			.ToTable("Users", Schemas.AUTHENTICATION)
			.HasKey(user => user.Id);

		builder
			.HasIndex(user => user.Email)
			.IsUnique();

		builder
			.HasMany(user => user.Workouts)
			.WithOne()
			.HasForeignKey(workout => workout.OwnerId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(user => user.ExerciseInfos)
			.WithOne()
			.HasForeignKey(exerciseInfo => exerciseInfo.OwnerId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasMany(user => user.TrackedWorkouts)
			.WithOne()
			.HasForeignKey(trackedWorkout => trackedWorkout.OwnerId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
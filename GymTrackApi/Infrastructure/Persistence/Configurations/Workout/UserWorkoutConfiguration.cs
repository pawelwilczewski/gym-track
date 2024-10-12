using Domain.Models.Workout;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class UserWorkoutConfiguration : IEntityTypeConfiguration<UserWorkout>
{
	public void Configure(EntityTypeBuilder<UserWorkout> builder)
	{
		builder.ToTable("UserWorkouts", Schemas.WORKOUT)
			.HasKey(userWorkout => new
			{
				userWorkout.UserId,
				userWorkout.WorkoutId
			});

		builder.HasOne(userWorkout => userWorkout.Workout)
			.WithMany(workout => workout.UserWorkouts)
			.HasForeignKey(userWorkout => userWorkout.UserId);
	}
}
using Domain.Models;
using Domain.Models.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Tracking;

internal sealed class TrackedWorkoutConfiguration : IEntityTypeConfiguration<TrackedWorkout>
{
	public void Configure(EntityTypeBuilder<TrackedWorkout> builder)
	{
		builder
			.ToTable("TrackedWorkouts", Schemas.TRACKING)
			.HasKey(trackedWorkout => trackedWorkout.Id);

		builder
			.HasOne(trackedWorkout => trackedWorkout.Workout)
			.WithMany(workout => workout.TrackedWorkouts)
			.HasForeignKey(trackedWorkout => trackedWorkout.WorkoutId);

		builder
			.Property(trackedWorkout => trackedWorkout.WorkoutId)
			.HasConversion(Id<Domain.Models.Workout.Workout>.Converter);

		builder
			.Property(trackedWorkout => trackedWorkout.Id)
			.HasConversion(Id<TrackedWorkout>.Converter);
	}
}
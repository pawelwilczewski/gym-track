using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class WorkoutConfiguration : IEntityTypeConfiguration<Domain.Models.Workout.Workout>
{
	public void Configure(EntityTypeBuilder<Domain.Models.Workout.Workout> builder)
	{
		builder
			.ToTable("Workouts", Schemas.WORKOUT)
			.HasKey(workout => workout.Id);
	}
}
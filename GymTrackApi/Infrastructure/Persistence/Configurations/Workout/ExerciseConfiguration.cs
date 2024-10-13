using Domain.Models.Workout;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
	public void Configure(EntityTypeBuilder<Exercise> builder)
	{
		builder
			.ToTable("WorkoutExercises", Schemas.WORKOUT)
			.HasKey(exercise => new
			{
				exercise.WorkoutId,
				ExerciseIndex = exercise.Index
			});

		builder
			.HasOne(exercise => exercise.Workout)
			.WithMany(workout => workout.Exercises)
			.HasForeignKey(exercise => exercise.WorkoutId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(exercise => exercise.ExerciseInfo)
			.WithMany(info => info.Exercises)
			.HasForeignKey(exercise => exercise.ExerciseInfoId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
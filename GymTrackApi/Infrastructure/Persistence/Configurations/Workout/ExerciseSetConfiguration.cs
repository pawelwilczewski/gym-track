using Domain.Models.Workout;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class ExerciseSetConfiguration : IEntityTypeConfiguration<ExerciseSet>
{
	public void Configure(EntityTypeBuilder<ExerciseSet> builder)
	{
		builder
			.ToTable("ExerciseSets", Schemas.WORKOUT)
			.HasKey(set => new
			{
				set.WorkoutId,
				set.ExerciseIndex,
				set.SetIndex
			});

		builder
			.HasOne(set => set.Exercise)
			.WithMany(exercise => exercise.Sets)
			.HasForeignKey(set => new
			{
				set.WorkoutId,
				set.ExerciseIndex
			});
	}
}
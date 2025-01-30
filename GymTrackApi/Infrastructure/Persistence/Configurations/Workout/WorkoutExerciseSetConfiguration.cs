using System.Text.Json;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;
using Infrastructure.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class WorkoutExerciseSetConfiguration : IEntityTypeConfiguration<WorkoutExerciseSet>
{
	public void Configure(EntityTypeBuilder<WorkoutExerciseSet> builder)
	{
		builder
			.ToTable("WorkoutExerciseSets", Schemas.WORKOUT)
			.HasKey(set => new
			{
				set.WorkoutId,
				set.ExerciseIndex,
				SetIndex = set.Index
			});

		builder
			.HasOne(set => set.Exercise)
			.WithMany(exercise => exercise.Sets)
			.HasForeignKey(set => new
			{
				set.WorkoutId,
				set.ExerciseIndex
			})
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(exerciseSet => exerciseSet.Metric)
			.HasConversion(
				metric => JsonSerializer.Serialize(metric, JsonSettings.Options),
				json => JsonSerializer.Deserialize<ExerciseMetric>(json, JsonSettings.Options)!)
			.HasColumnType("json");

		builder
			.Navigation(set => set.Exercise)
			.AutoInclude();
	}
}
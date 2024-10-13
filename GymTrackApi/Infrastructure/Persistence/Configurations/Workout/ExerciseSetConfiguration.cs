using System.Text.Json;
using Application.Serialization;
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

		builder.Property(exerciseSet => exerciseSet.Metric)
			.HasConversion(
				metric => JsonSerializer.Serialize(metric, JsonSettings.Options),
				json => JsonSerializer.Deserialize<ExerciseMetric>(json, JsonSettings.Options)!)
			.HasColumnType("jsonb");
	}
}
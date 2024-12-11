using Domain.Models.Workout;
using Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class ExerciseInfoStepConfiguration : IEntityTypeConfiguration<ExerciseInfo.Step>
{
	public void Configure(EntityTypeBuilder<ExerciseInfo.Step> builder)
	{
		const string tableName = "ExerciseInfoSteps";

		builder
			.ToTable(tableName, Schemas.WORKOUT)
			.HasKey(step => new
			{
				step.ExerciseInfoId,
				step.Index
			});

		builder.Property(step => step.Description).ConfigureDescription();

		builder.Property(step => step.ImageFile).ConfigureOptionalFilePath();

		builder
			.HasOne(step => step.ExerciseInfo)
			.WithMany(exerciseInfo => exerciseInfo.Steps)
			.HasForeignKey(step => step.ExerciseInfoId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
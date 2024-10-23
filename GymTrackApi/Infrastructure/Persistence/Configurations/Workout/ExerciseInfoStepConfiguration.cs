using Domain.Models.Workout;
using Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class ExerciseInfoStepConfiguration : IEntityTypeConfiguration<ExerciseInfo.Step>
{
	public void Configure(EntityTypeBuilder<ExerciseInfo.Step> builder)
	{
		builder
			.ToTable("ExerciseInfoSteps", Schemas.WORKOUT)
			.HasKey(step => new
			{
				step.ExerciseInfoId,
				step.Index
			});

		builder.Property(step => step.Description).ConfigureDescription();

		builder.Property(step => step.ImageFile).ConfigureOptionalFilePath();

		builder
			.HasOne(stepInfo => stepInfo.ExerciseInfo)
			.WithMany(exerciseInfo => exerciseInfo.Steps)
			.HasForeignKey(stepInfo => stepInfo.ExerciseInfoId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
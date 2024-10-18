using Domain.Models.Workout;
using Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class ExerciseStepInfoConfiguration : IEntityTypeConfiguration<ExerciseStepInfo>
{
	public void Configure(EntityTypeBuilder<ExerciseStepInfo> builder)
	{
		builder
			.ToTable("ExerciseStepInfos", Schemas.WORKOUT)
			.HasKey(step => new
			{
				step.ExerciseInfoId,
				step.Index
			});

		builder.Property(step => step.Description).ConfigureDescription();

		builder.ComplexProperty(step => step.ImageFile)
			.Configure(new OptionalFilePathConfiguration());

		builder
			.HasOne(stepInfo => stepInfo.ExerciseInfo)
			.WithMany(exerciseInfo => exerciseInfo.Steps)
			.HasForeignKey(stepInfo => stepInfo.ExerciseInfoId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
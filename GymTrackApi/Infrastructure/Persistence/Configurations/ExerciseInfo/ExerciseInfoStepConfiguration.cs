using Domain.Models.ExerciseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.ExerciseInfo;

internal sealed class ExerciseInfoStepConfiguration : IEntityTypeConfiguration<ExerciseInfoStep>
{
	public void Configure(EntityTypeBuilder<ExerciseInfoStep> builder)
	{
		const string tableName = "ExerciseInfoSteps";

		builder
			.ToTable(tableName, Schemas.WORKOUT)
			.HasKey(step => new
			{
				step.ExerciseInfoId,
				step.Index
			});

		builder
			.HasOne(step => step.ExerciseInfo)
			.WithMany(exerciseInfo => exerciseInfo.Steps)
			.HasForeignKey(step => step.ExerciseInfoId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.Navigation(step => step.ExerciseInfo)
			.AutoInclude();
	}
}
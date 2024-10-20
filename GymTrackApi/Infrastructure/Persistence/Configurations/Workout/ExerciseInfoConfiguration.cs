using Domain.Models;
using Domain.Models.Workout;
using Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class ExerciseInfoConfiguration : IEntityTypeConfiguration<ExerciseInfo>
{
	public void Configure(EntityTypeBuilder<ExerciseInfo> builder)
	{
		builder.ToTable("ExerciseInfos", Schemas.WORKOUT)
			.HasKey(exerciseInfo => exerciseInfo.Id);

		builder.Property(exerciseInfo => exerciseInfo.Id)
			.HasConversion(Id<ExerciseInfo>.Converter);

		builder.Property(info => info.ThumbnailImage).ConfigureFilePath();

		builder.Property(info => info.Name).ConfigureName();
		builder.Property(info => info.Description).ConfigureDescription();
	}
}
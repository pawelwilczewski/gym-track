using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.ExerciseInfo;

internal sealed class ExerciseInfoConfiguration : IEntityTypeConfiguration<Domain.Models.ExerciseInfo.ExerciseInfo>
{
	public void Configure(EntityTypeBuilder<Domain.Models.ExerciseInfo.ExerciseInfo> builder)
	{
		builder.ToTable("ExerciseInfos", Schemas.WORKOUT)
			.HasKey(exerciseInfo => exerciseInfo.Id);

		builder.Property(exerciseInfo => exerciseInfo.Id)
			.HasConversion(Id<Domain.Models.ExerciseInfo.ExerciseInfo>.Converter);
	}
}
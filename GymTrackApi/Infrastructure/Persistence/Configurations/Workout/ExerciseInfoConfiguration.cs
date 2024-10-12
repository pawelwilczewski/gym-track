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
			.HasKey(exercise => exercise.Id);

		builder.Property(exercise => exercise.Id)
			.HasConversion(id => id.Value, value => new Id<ExerciseInfo>(value));

		builder.Property(info => info.Name)
			.HasMaxLength(50);

		builder.ComplexProperty(info => info.ThumbnailImage)
			.Configure(new FilePathConfiguration());
	}
}
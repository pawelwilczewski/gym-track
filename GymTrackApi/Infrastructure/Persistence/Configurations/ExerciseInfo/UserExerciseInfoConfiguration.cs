using Domain.Models;
using Domain.Models.ExerciseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.ExerciseInfo;

internal sealed class UserExerciseInfoConfiguration : IEntityTypeConfiguration<UserExerciseInfo>
{
	public void Configure(EntityTypeBuilder<UserExerciseInfo> builder)
	{
		builder.ToTable("UserExerciseInfos", Schemas.WORKOUT)
			.HasKey(userWorkout => new
			{
				userWorkout.UserId,
				userWorkout.ExerciseInfoId
			});

		builder.Property(userExerciseInfo => userExerciseInfo.ExerciseInfoId)
			.HasConversion(Id<Domain.Models.ExerciseInfo.ExerciseInfo>.Converter);

		builder.HasOne(userExerciseInfo => userExerciseInfo.ExerciseInfo)
			.WithMany(exerciseInfo => exerciseInfo.Users)
			.HasForeignKey(userExerciseInfo => userExerciseInfo.ExerciseInfoId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
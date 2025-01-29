using Domain.Common.ValueObjects;
using Domain.Models;
using Domain.Models.ExerciseInfo;
using Domain.Models.Workout;
using Microsoft.EntityFrameworkCore;
using Vogen;

namespace Infrastructure.Persistence.Configurations;

internal static class PropertyConfigurations
{
	public static ModelConfigurationBuilder ConfigureProperties(this ModelConfigurationBuilder builder)
	{
		builder.RegisterAllInVogenEfCoreConverters();

		builder.Properties<Name>().HaveMaxLength(Name.MAX_LENGTH);
		builder.Properties<Description>().HaveMaxLength(Description.MAX_LENGTH);
		builder.Properties<FilePath>().HaveMaxLength(FilePath.MAX_LENGTH);

		return builder;
	}
}

[EfCoreConverter<WorkoutId>]
[EfCoreConverter<ExerciseInfoId>]
[EfCoreConverter<Name>]
[EfCoreConverter<Description>]
[EfCoreConverter<FilePath>]
[EfCoreConverter<Reps>]
internal static partial class VogenEfCoreConverters;
using Domain.Common.ValueObjects;
using Infrastructure.Persistence.Configurations.Converters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations;

internal static class PropertyConfigurations
{
	public static ModelConfigurationBuilder ConfigureProperties(this ModelConfigurationBuilder builder)
	{
		builder.RegisterAllInVogenConverters();

		builder.Properties<Name>().HaveMaxLength(Name.MAX_LENGTH);
		builder.Properties<Description>().HaveMaxLength(Description.MAX_LENGTH);
		builder.Properties<FilePath>().HaveMaxLength(FilePath.MAX_LENGTH);
		builder.Properties<PasswordHash>().HaveMaxLength(PasswordHash.BYTES_LENGTH * 2 + 1);
		builder.Properties<EmailAddress>().HaveMaxLength(EmailAddress.MAX_LENGTH);
		builder.Properties<EmailConfirmationCode>().HaveMaxLength(EmailConfirmationCode.BYTES_LENGTH * 2);

		builder.Properties<SomeExerciseMetricTypes>().HaveConversion<SomeExerciseMetricTypesConverter>();
		builder.Properties<SingleExerciseMetricType>().HaveConversion<SingleExerciseMetricTypeConverter>();

		return builder;
	}
}
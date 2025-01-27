using Domain.Common.Ownership;
using Domain.Common.ValueObjects;
using Infrastructure.Persistence.Configurations.ValueConverters;
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

		builder.Properties<Owner>().HaveConversion<OwnerValueConverter>();

		return builder;
	}
}

[EfCoreConverter<Name>]
[EfCoreConverter<Description>]
[EfCoreConverter<FilePath>]
internal static partial class VogenEfCoreConverters;
using Domain.Common;
using Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class FilePathConfiguration : IComplexTypePropertyConfiguration<FilePath>
{
	public const int MAX_LENGTH = 500;

	public ComplexPropertyBuilder<FilePath> Configure(ComplexPropertyBuilder<FilePath> builder)
	{
		builder.Property(path => path.Path)
			.HasMaxLength(MAX_LENGTH);

		return builder;
	}
}

internal sealed class OptionalFilePathConfiguration : IComplexTypePropertyConfiguration<OptionalFilePath>
{
	public ComplexPropertyBuilder<OptionalFilePath> Configure(ComplexPropertyBuilder<OptionalFilePath> builder)
	{
		builder.Property(path => path.Path)
			.HasMaxLength(FilePathConfiguration.MAX_LENGTH);

		return builder;
	}
}
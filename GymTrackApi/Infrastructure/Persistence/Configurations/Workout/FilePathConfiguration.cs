using Domain.Common;
using Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Workout;

internal sealed class FilePathConfiguration : IComplexTypePropertyConfiguration<FilePath>
{
	public ComplexPropertyBuilder<FilePath> Configure(ComplexPropertyBuilder<FilePath> builder) => builder;
}

internal sealed class OptionalFilePathConfiguration : IComplexTypePropertyConfiguration<OptionalFilePath>
{
	public ComplexPropertyBuilder<OptionalFilePath> Configure(ComplexPropertyBuilder<OptionalFilePath> builder) => builder;
}
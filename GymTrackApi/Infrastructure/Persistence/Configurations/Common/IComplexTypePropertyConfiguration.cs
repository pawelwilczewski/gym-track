using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Common;

internal interface IComplexTypePropertyConfiguration<T>
{
	ComplexPropertyBuilder<T> Configure(ComplexPropertyBuilder<T> builder);
}

internal static class ConfigurationExtensions
{
	public static ComplexPropertyBuilder<T> Configure<T>(
		this ComplexPropertyBuilder<T> builder,
		IComplexTypePropertyConfiguration<T> configuration) =>
		configuration.Configure(builder);
}
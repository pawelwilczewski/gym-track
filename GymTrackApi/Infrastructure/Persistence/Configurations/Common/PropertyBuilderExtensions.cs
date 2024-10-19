using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Common;

internal static class PropertyBuilderExtensions
{
	public static PropertyBuilder<TProperty> ConfigureName<TProperty>(this PropertyBuilder<TProperty> builder) =>
		builder
			.HasConversion(Name.Converter, Name.Comparer)
			.HasMaxLength(Name.MAX_LENGTH);

	public static PropertyBuilder<TProperty> ConfigureDescription<TProperty>(this PropertyBuilder<TProperty> builder) =>
		builder
			.HasConversion(Description.Converter, Description.Comparer)
			.HasMaxLength(Description.MAX_LENGTH);
}
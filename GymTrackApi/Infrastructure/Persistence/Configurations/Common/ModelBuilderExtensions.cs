using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Configurations.Common;

internal static class ModelBuilderExtensions
{
	public static void RegisterConfigurationsInAssembly(this ModelBuilder builder)
	{
		var typesToRegister = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(type => type.GetInterfaces()
				.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

		foreach (var type in typesToRegister)
		{
			var entityType = type.GetInterfaces()
				.First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
				.GetGenericArguments()[0];

			var applyConfigMethod = typeof(ModelBuilder)
				.GetMethod(nameof(ModelBuilder.ApplyConfiguration))
				?.MakeGenericMethod(entityType);

			var configurationInstance = Activator.CreateInstance(type);

			applyConfigMethod?.Invoke(builder, [configurationInstance]);
		}
	}
}
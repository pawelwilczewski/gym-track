namespace Api.Common;

internal static class HostEnvironmentExtensions
{
	public static bool IsTest(this IHostEnvironment hostEnvironment) =>
		hostEnvironment.IsEnvironment("Test");

	public static bool IsDevelopmentOrTest(this IHostEnvironment hostEnvironment) =>
		hostEnvironment.IsDevelopment() || hostEnvironment.IsTest();

	public static bool IsProductionOrTest(this IHostEnvironment hostEnvironment) =>
		hostEnvironment.IsProduction() || hostEnvironment.IsTest();
}
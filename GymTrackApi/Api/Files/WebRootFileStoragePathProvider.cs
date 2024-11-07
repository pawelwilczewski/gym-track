using Application.Persistence;

namespace Api.Files;

internal sealed class WebRootFileStoragePathProvider : IFileStoragePathProvider
{
	public string RootPath => environment.WebRootPath;

	private readonly IWebHostEnvironment environment;

	public WebRootFileStoragePathProvider(IWebHostEnvironment environment) => this.environment = environment;
}
using Application.Persistence;

namespace Api.Tests.Unit.Mocks;

internal sealed class TempFileStoragePathProvider : IFileStoragePathProvider
{
	public string RootPath => Path.Combine(Path.GetTempPath(), "GymTrack-Test");
}
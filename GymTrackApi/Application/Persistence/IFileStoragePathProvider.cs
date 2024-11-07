namespace Application.Persistence;

public interface IFileStoragePathProvider
{
	string RootPath { get; }
}
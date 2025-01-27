namespace Application.Persistence;

public interface IUserDataContextFactory : IDisposable
{
	IUserDataContext ForUser(Guid userId);
}
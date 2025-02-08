using Domain.Models.User;

namespace Application.Persistence;

public interface IUserDataContextFactory : IDisposable
{
	IUserDataContext ForUser(UserId UserId);
}
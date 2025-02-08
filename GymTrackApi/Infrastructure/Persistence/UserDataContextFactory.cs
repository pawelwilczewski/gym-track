using Application.Persistence;
using Domain.Models.User;

namespace Infrastructure.Persistence;

internal sealed class UserDataContextFactory : IUserDataContextFactory, IAsyncDisposable
{
	private readonly AppDbContext dbContext;

	public UserDataContextFactory(AppDbContext dbContext) => this.dbContext = dbContext;

	public IUserDataContext ForUser(UserId userId) => new UserDataContext(userId, dbContext);

	public ValueTask DisposeAsync() => dbContext.DisposeAsync();
	public void Dispose() => dbContext.Dispose();
}
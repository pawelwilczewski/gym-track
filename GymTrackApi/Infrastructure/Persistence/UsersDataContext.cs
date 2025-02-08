using Application.Persistence;
using Domain.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class UsersDataContext : IUsersDataContext
{
	public DbSet<User> Users => dbContext.Users;

	private readonly AppDbContext dbContext;

	public UsersDataContext(AppDbContext dbContext) =>
		this.dbContext = dbContext;

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
		dbContext.SaveChangesAsync(cancellationToken);

	public ValueTask DisposeAsync() => dbContext.DisposeAsync();

	public void Dispose() => dbContext.Dispose();
}
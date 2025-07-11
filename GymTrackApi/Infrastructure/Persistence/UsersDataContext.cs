using Application.Persistence;
using Domain.Common.Results;
using Domain.Models.User;
using FuncNet;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class UsersDataContext : IUsersDataContext
{
	public DbSet<User> Users => dbContext.Users;

	private readonly AppDbContext dbContext;

	public UsersDataContext(AppDbContext dbContext) =>
		this.dbContext = dbContext;

	public async Task<Result<Success, DatabaseError>> SaveChangesAsync(CancellationToken cancellationToken)
	{
		try
		{
			await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
			return Success.Instance;
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}

	public ValueTask DisposeAsync() => dbContext.DisposeAsync();

	public void Dispose() => dbContext.Dispose();
}
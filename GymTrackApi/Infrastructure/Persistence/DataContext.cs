using Application.Persistence;
using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal sealed class DataContext : IDataContext
{
	public DbSet<User> Users => dbContext.Users;

	private readonly AppDbContext dbContext;

	public DataContext(AppDbContext dbContext) => this.dbContext = dbContext;

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
		dbContext.SaveChangesAsync(cancellationToken);

	public void Dispose() => dbContext.Dispose();
}
using Domain.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Application.Persistence;

public interface IUsersDataContext : IDisposable, IAsyncDisposable
{
	DbSet<User> Users { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
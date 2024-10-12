using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Persistence;

public interface IDataContext : IDisposable
{
	DbSet<User> Users { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
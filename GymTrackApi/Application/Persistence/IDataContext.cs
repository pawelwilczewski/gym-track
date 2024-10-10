using Domain.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Application.Persistence;

public interface IDataContext : IDisposable
{
	DbSet<AppUser> Users { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
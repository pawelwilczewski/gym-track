using Domain.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Persistence;

public interface IDataContext : IDisposable
{
	DbSet<AppUser> Users { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
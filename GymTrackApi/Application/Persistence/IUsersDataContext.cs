using Domain.Common.Results;
using Domain.Models.User;
using FuncNet;
using Microsoft.EntityFrameworkCore;

namespace Application.Persistence;

public interface IUsersDataContext : IDisposable, IAsyncDisposable
{
	DbSet<User> Users { get; }
	Task<Result<Success, DatabaseError>> SaveChangesAsync(CancellationToken cancellationToken);
}
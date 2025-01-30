using Application.Persistence;
using Domain.Common.Exceptions;
using Domain.Common.Ownership;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence;

internal sealed class UserAssetRepository<TEntity> : IUserAssetRepository<TEntity> where TEntity : class, IOwned
{
	public IQueryable<TEntity> Readable => dbContext.Set<TEntity>()
		.Where(entity => entity.OwnerId == null || entity.OwnerId == userId);

	public IQueryable<TEntity> Modifiable => dbContext.Set<TEntity>()
		.Where(entity => entity.OwnerId == userId);

	private readonly Guid userId;
	private readonly AppDbContext dbContext;

	public UserAssetRepository(Guid userId, AppDbContext dbContext)
	{
		this.userId = userId;
		this.dbContext = dbContext;
	}

	public EntityEntry<TEntity> Add(TEntity entity)
	{
		if (entity.Owner is Owner.Public) throw new InvalidOperationException("Currently adding public entities is not supported.");
		if (!entity.CanBeModifiedBy(userId)) throw new PermissionError();

		return dbContext.Add(entity);
	}

	public EntityEntry<TEntity> Remove(TEntity entity)
	{
		if (entity.Owner is Owner.Public) throw new InvalidOperationException("Currently removing public entities is not supported.");
		if (!entity.CanBeModifiedBy(userId)) throw new PermissionError();

		return dbContext.Remove(entity);
	}
}